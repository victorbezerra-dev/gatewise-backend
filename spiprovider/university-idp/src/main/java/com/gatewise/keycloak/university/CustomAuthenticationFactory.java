package com.gatewise.keycloak.university;

import java.util.Collections;
import java.util.List;

import org.jboss.logging.Logger;
import org.keycloak.Config;
import org.keycloak.authentication.Authenticator;
import org.keycloak.authentication.AuthenticatorFactory;
import org.keycloak.models.AuthenticationExecutionModel;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.KeycloakSessionFactory;
import org.keycloak.provider.ProviderConfigProperty;

public class CustomAuthenticationFactory implements AuthenticatorFactory {

    private static final Logger LOG = Logger.getLogger(CustomAuthenticationFactory.class);

    private static final CustomAuthenticator SINGLETON = new CustomAuthenticator();

    @Override
    public Authenticator create(KeycloakSession keycloakSession) {
        LOG.info("[GatewiseCustomAuthenticatorFactory] create() invoked. Returning custom authenticator singleton.");
        return SINGLETON;
    }

    @Override
    public AuthenticationExecutionModel.Requirement[] getRequirementChoices() {
        return new AuthenticationExecutionModel.Requirement[]{
                AuthenticationExecutionModel.Requirement.REQUIRED,
                AuthenticationExecutionModel.Requirement.DISABLED
        };
    }

    @Override
    public String getDisplayType() {
        return "Gatewise Custom Authenticator";
    }

    @Override
    public String getReferenceCategory() {
        return "password";
    }

    @Override
    public boolean isConfigurable() {
        return false;
    }

    @Override
    public boolean isUserSetupAllowed() {
        return false;
    }

    @Override
    public String getHelpText() {
        return "Custom authentication with external API using university credentials";
    }

    @Override
    public List<ProviderConfigProperty> getConfigProperties() {
        return Collections.emptyList();
    }

    @Override
    public void init(Config.Scope scope) {
        LOG.info("[GatewiseCustomAuthenticatorFactory] init() invoked. Provider loaded by Keycloak.");
    }

    @Override
    public void postInit(KeycloakSessionFactory keycloakSessionFactory) {
        LOG.info("[GatewiseCustomAuthenticatorFactory] postInit() invoked. Provider is ready.");
    }

    @Override
    public void close() {
    }

    @Override
    public String getId() {
        return CustomAuthenticator.CUSTOM_AUTHENTICATOR_PROVIDER_ID;
    }
}
