package com.gatewise.keycloak.university.repositories;

import java.io.IOException;
import java.nio.charset.StandardCharsets;

import org.apache.http.HttpHeaders;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.ContentType;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;
import org.jboss.logging.Logger;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.gatewise.keycloak.university.dto.AuthRequestDTO;
import com.gatewise.keycloak.university.dto.AuthResponseDTO;

public class CustomExternalApi {

    private static final Logger LOG = Logger.getLogger(CustomExternalApi.class);

    public static final String API_URL = getRequiredEnv("API_URL");
    public static final String API_AUTH_KEY = getRequiredEnv("API_AUTH_KEY");

    private final ObjectMapper objectMapper = new ObjectMapper();

    private static String getRequiredEnv(String name) {
        String value = System.getenv(name);
        if (value == null || value.trim().isEmpty()) {
            throw new IllegalStateException("Environment variable " + name + " is required for CustomExternalApi.");
        }
        return value.trim();
    }

    public AuthResponseDTO loginAndGetUserInfo(String username, String password) throws IOException {

        try (CloseableHttpClient httpClient = HttpClients.createDefault()) {
            LOG.infof("[CustomExternalApi] HttpClient created. username=%s", username);

            String endpoint = API_URL;
            HttpPost httpPost = new HttpPost(endpoint);
            httpPost.setHeader(HttpHeaders.AUTHORIZATION, "key=" + API_AUTH_KEY);

            LOG.infof("[CustomExternalApi] Building JSON payload. username=%s", username);
            String json = objectMapper.writeValueAsString(new AuthRequestDTO(username, password));

            httpPost.setEntity(new StringEntity(json, ContentType.APPLICATION_JSON));
            LOG.infof("[CustomExternalApi] Sending request to external API. endpoint=%s username=%s", endpoint, username);

            try (CloseableHttpResponse response = httpClient.execute(httpPost)) {
                LOG.infof("[CustomExternalApi] Response received from API. username=%s", username);
                int statusCode = response.getStatusLine().getStatusCode();
                String responseJson = "";
                if (response.getEntity() != null) {
                    responseJson = EntityUtils.toString(response.getEntity(), StandardCharsets.UTF_8);
                }
                LOG.infof("[CustomExternalApi] Status code received. status=%d username=%s responseBodyLength=%d", statusCode, username, responseJson.length());

                if (statusCode == 200) {
                    AuthResponseDTO dto = objectMapper.readValue(responseJson, AuthResponseDTO.class);
                    LOG.infof("[CustomExternalApi] Authentication successful according to external API. username=%s authenticated=%s", username, dto.isAuthenticated());
                    return dto;
                } else {
                    LOG.warnf("[CustomExternalApi] Authentication failed according to external API. status=%d username=%s", statusCode, username);
                    return null;
                }
            } catch (Exception e) {
                LOG.errorf(e, "[CustomExternalApi] Error while executing HTTP request. username=%s", username);
                throw e;
            }
        } catch (Exception e) {
            LOG.errorf(e, "[CustomExternalApi] General error in loginAndGetUserInfo. username=%s", username);
            throw e instanceof IOException ? (IOException) e : new IOException(e);
        }
    }
}
