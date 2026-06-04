package com.gatewise.keycloak.university;

import java.io.IOException;
import java.util.Arrays;
import java.util.Objects;

import javax.sql.DataSource;

import org.jboss.logging.Logger;
import org.keycloak.authentication.AuthenticationFlowContext;
import org.keycloak.authentication.AuthenticationFlowError;
import org.keycloak.authentication.Authenticator;
import org.keycloak.authentication.authenticators.browser.UsernamePasswordForm;
import org.keycloak.models.ClientModel;
import org.keycloak.models.RealmModel;
import org.keycloak.models.RoleModel;
import org.keycloak.models.UserModel;

import com.gatewise.keycloak.university.dto.AuthResponseDTO;
import com.gatewise.keycloak.university.dto.AuthResponseDTO.AffiliationType;
import com.gatewise.keycloak.university.dto.AuthResponseDTO.Vinculo;
import com.gatewise.keycloak.university.repositories.CustomExternalApi;
import com.gatewise.keycloak.university.repositories.OutboxRepository;
import com.gatewise.keycloak.university.utils.DataSourceProvider;
import com.gatewise.keycloak.university.utils.UserEventBuilder;

import jakarta.ws.rs.core.MultivaluedMap;

public class CustomAuthenticator extends UsernamePasswordForm implements Authenticator {

    private static final Logger LOG = Logger.getLogger(CustomAuthenticator.class);

    public static final String CUSTOM_AUTHENTICATOR_PROVIDER_ID = "university-idp";

    @Override
    public void authenticate(AuthenticationFlowContext context) {
        LOG.infof(
                "[GatewiseCustomAuthenticator] authenticate() invoked. realm=%s clientId=%s execution=%s remoteAddr=%s",
                getRealmName(context),
                getClientId(context),
                context.getExecution() != null ? context.getExecution().getId() : "unknown",
                getRemoteAddress(context)
        );

        context.challenge(challenge(context, null, null));
    }

    @Override
    public void action(AuthenticationFlowContext context) {
        LOG.infof(
                "[GatewiseCustomAuthenticator] action() invoked. realm=%s clientId=%s remoteAddr=%s",
                getRealmName(context),
                getClientId(context),
                getRemoteAddress(context)
        );

        MultivaluedMap<String, String> formData = context.getHttpRequest().getDecodedFormParameters();
        String username = formData.getFirst("username");
        String password = formData.getFirst("password");

        if (username == null || password == null) {
            LOG.warnf(
                    "[GatewiseCustomAuthenticator] Missing credentials in submitted form. realm=%s clientId=%s hasUsername=%s hasPassword=%s",
                    getRealmName(context),
                    getClientId(context),
                    username != null,
                    password != null
            );
            fail(context, AuthenticationFlowError.INVALID_USER, "Usuário ou senha não informados!");
            return;
        }

        RealmModel realm = context.getRealm();
        UserModel user = context.getSession().users().getUserByUsername(realm, username);

        LOG.infof(
                "[GatewiseCustomAuthenticator] Login attempt received. realm=%s clientId=%s username=%s existingUser=%s",
                realm.getName(),
                getClientId(context),
                username,
                user != null
        );

        RoleModel adminRole = realm.getRole("admin");
        if (user != null && adminRole != null && user.hasRole(adminRole)) {
            LOG.infof(
                    "[GatewiseCustomAuthenticator] Existing admin user detected. Validating with Keycloak local password. username=%s realm=%s",
                    username,
                    realm.getName()
            );
            if (validatePassword(context, user, formData, false)) {
                succeed(context, user);
            } else {
                fail(context, AuthenticationFlowError.INVALID_CREDENTIALS, "Senha inválida para admin!");
            }
            return;
        }

        AuthResponseDTO response = tryAuthenticateExternal(context, username, password);
        if (response == null || !response.isAuthenticated()) {
            LOG.warnf(
                    "[GatewiseCustomAuthenticator] External authentication did not authenticate user. username=%s realm=%s clientId=%s responsePresent=%s authenticated=%s",
                    username,
                    realm.getName(),
                    getClientId(context),
                    response != null,
                    response != null && response.isAuthenticated()
            );
            return;
        }

        LOG.infof(
                "[GatewiseCustomAuthenticator] External authentication succeeded. username=%s realm=%s clientId=%s affiliationType=%s",
                username,
                realm.getName(),
                getClientId(context),
                response.getVinculo() != null ? response.getVinculo().getTipo() : "null"
        );

        if (user == null) {
            LOG.infof(
                    "[GatewiseCustomAuthenticator] User does not exist in Keycloak. Creating user. username=%s realm=%s",
                    username,
                    realm.getName()
            );
            user = createUser(context, realm, username, response);
        } else {
            LOG.infof(
                    "[GatewiseCustomAuthenticator] User already exists in Keycloak. Updating attributes/roles. username=%s userId=%s realm=%s",
                    username,
                    user.getId(),
                    realm.getName()
            );
        }

        assignAttributes(user, response);
        assignRoles(user, context.getRealm(), response.getVinculo());

        tryPublishOrOutbox(user, realm);

        succeed(context, user);
    }

    private void fail(AuthenticationFlowContext context, AuthenticationFlowError error, String message) {
        LOG.warnf(
                "[GatewiseCustomAuthenticator] Authentication failed. realm=%s clientId=%s error=%s message=%s",
                getRealmName(context),
                getClientId(context),
                error,
                message
        );
        context.failureChallenge(error, context.form().setError(message).createLoginUsernamePassword());
    }

    private void succeed(AuthenticationFlowContext context, UserModel user) {
        LOG.infof(
                "[GatewiseCustomAuthenticator] Authentication succeeded. realm=%s clientId=%s username=%s userId=%s",
                getRealmName(context),
                getClientId(context),
                user != null ? user.getUsername() : "null",
                user != null ? user.getId() : "null"
        );
        context.setUser(user);
        context.success();
    }

    private AuthResponseDTO tryAuthenticateExternal(AuthenticationFlowContext context, String username, String password) {
        try {
            LOG.infof(
                    "[GatewiseCustomAuthenticator] Calling external authentication API. username=%s realm=%s clientId=%s",
                    username,
                    getRealmName(context),
                    getClientId(context)
            );
            return new CustomExternalApi().loginAndGetUserInfo(username, password);
        } catch (IOException e) {
            LOG.errorf(
                    e,
                    "[GatewiseCustomAuthenticator] External authentication API call failed. username=%s realm=%s clientId=%s",
                    username,
                    getRealmName(context),
                    getClientId(context)
            );
            fail(context, AuthenticationFlowError.INTERNAL_ERROR, "Erro interno ao validar usuário!");
            return null;
        }
    }

    private UserModel createUser(AuthenticationFlowContext context, RealmModel realm, String username, AuthResponseDTO response) {
        UserModel user = context.getSession().users().addUser(realm, username);
        user.setEnabled(true);

        String fullName = response.getNome() != null ? response.getNome().trim() : "";
        String[] parts = fullName.split("\\s+");

        user.setFirstName(parts.length > 0 ? parts[0] : "");
        user.setLastName(parts.length > 1 ? String.join(" ", Arrays.copyOfRange(parts, 1, parts.length)) : "");

        String email = response.getVinculo() != null ? Objects.toString(response.getVinculo().getEmail(), "") : "";

        user.setEmail(email);
        user.setUsername(username);

        LOG.infof(
                "[GatewiseCustomAuthenticator] User created in Keycloak. username=%s userId=%s realm=%s emailPresent=%s",
                username,
                user.getId(),
                realm.getName(),
                email != null && !email.isBlank()
        );

        return user;
    }

    private void assignRoles(UserModel user, RealmModel realm, Vinculo vinculo) {
        if (vinculo == null) {
            LOG.warnf(
                    "[GatewiseCustomAuthenticator] Cannot assign role because affiliation/vinculo is null. username=%s realm=%s",
                    user.getUsername(),
                    realm.getName()
            );
            return;
        }

        String roleName = null;
        switch (vinculo.getTipo()) {
            case AffiliationType.PROFESSOR:
            case AffiliationType.SERVICE_PROVIDER_PROFESSOR:
                roleName = "role_professor";
                break;

            case AffiliationType.STUDENT:
                roleName = "role_student";
                break;

            case AffiliationType.VISITOR:
                roleName = "role_visitor";
                break;

            default:
                break;
        }

        if (roleName == null) {
            LOG.warnf(
                    "[GatewiseCustomAuthenticator] No Keycloak role mapping for affiliation type. username=%s realm=%s affiliationType=%s",
                    user.getUsername(),
                    realm.getName(),
                    vinculo.getTipo()
            );
            return;
        }

        RoleModel role = realm.getRole(roleName);
        if (role == null) {
            LOG.warnf(
                    "[GatewiseCustomAuthenticator] Expected Keycloak role does not exist. username=%s realm=%s role=%s",
                    user.getUsername(),
                    realm.getName(),
                    roleName
            );
            return;
        }

        user.grantRole(role);
        LOG.infof(
                "[GatewiseCustomAuthenticator] Role assigned. username=%s realm=%s role=%s affiliationType=%s",
                user.getUsername(),
                realm.getName(),
                roleName,
                vinculo.getTipo()
        );

    }

    private void assignAttributes(UserModel user, AuthResponseDTO response) {
        Vinculo vinculo = response.getVinculo();
        if (vinculo == null) {
            LOG.warnf(
                    "[GatewiseCustomAuthenticator] Cannot assign custom attributes because affiliation/vinculo is null. username=%s",
                    user.getUsername()
            );
            return;
        }

        String fullName = Objects.toString(response.getNome(), "").trim();
        user.setSingleAttribute("custom.name", fullName);
        user.setSingleAttribute("custom.description", Objects.toString(vinculo.getDescricao(), ""));
        user.setSingleAttribute("custom.registration", Objects.toString(vinculo.getMatricula(), ""));
        user.setSingleAttribute("custom.email", Objects.toString(vinculo.getEmail(), ""));
        user.setSingleAttribute("custom.photo", Objects.toString(vinculo.getFoto(), ""));
        user.setSingleAttribute("custom.unitId", String.valueOf(vinculo.getUnidadeId()));
        user.setSingleAttribute("custom.unitName", Objects.toString(vinculo.getNomeUnidade(), ""));
        user.setSingleAttribute("custom.courseId", String.valueOf(vinculo.getCursoId()));
        user.setSingleAttribute("custom.entryYear", String.valueOf(vinculo.getAnoIngresso()));
        user.setSingleAttribute("custom.statusDescription", Objects.toString(vinculo.getDescricaoSituacao(), ""));
        user.setSingleAttribute("custom.userType", String.valueOf(vinculo.getTipo()));

        LOG.infof(
                "[GatewiseCustomAuthenticator] Custom attributes assigned. username=%s unitId=%s courseId=%s affiliationType=%s",
                user.getUsername(),
                vinculo.getUnidadeId(),
                vinculo.getCursoId(),
                vinculo.getTipo()
        );
    }

    private void tryPublishOrOutbox(UserModel user, RealmModel realm) {
        RoleModel adminRole = realm.getRole("admin");
        if (adminRole != null && user.hasRole(adminRole)) {
            LOG.infof(
                    "[GatewiseCustomAuthenticator] Skipping user logged-in event for admin user. username=%s realm=%s",
                    user.getUsername(),
                    realm.getName()
            );
            return;
        }
        
        String json = UserEventBuilder.buildUserLoggedInEvent(user);
        RabbitMQPublisher publisher = new RabbitMQPublisher();
        boolean published = publisher.tryPublish(json);
        if (!published) {
            LOG.warnf(
                    "[GatewiseCustomAuthenticator] RabbitMQ publish failed. Saving event to outbox. username=%s realm=%s",
                    user.getUsername(),
                    realm.getName()
            );
            DataSource dataSource = DataSourceProvider.get();
            OutboxRepository outbox = new OutboxRepository(dataSource);
            outbox.save("USER_LOGGED_IN", json);
        } else {
            LOG.infof(
                    "[GatewiseCustomAuthenticator] User logged-in event published to RabbitMQ. username=%s realm=%s",
                    user.getUsername(),
                    realm.getName()
            );
        }
    }

    private String getRealmName(AuthenticationFlowContext context) {
        return context != null && context.getRealm() != null ? context.getRealm().getName() : "unknown";
    }

    private String getClientId(AuthenticationFlowContext context) {
        ClientModel client = context != null && context.getAuthenticationSession() != null
                ? context.getAuthenticationSession().getClient()
                : null;
        return client != null ? client.getClientId() : "unknown";
    }

    private String getRemoteAddress(AuthenticationFlowContext context) {
        return context != null && context.getConnection() != null
                ? context.getConnection().getRemoteAddr()
                : "unknown";
    }
}
