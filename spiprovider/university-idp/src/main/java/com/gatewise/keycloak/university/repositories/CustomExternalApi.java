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

import com.fasterxml.jackson.databind.ObjectMapper;
import com.gatewise.keycloak.university.dto.AuthRequestDTO;
import com.gatewise.keycloak.university.dto.AuthResponseDTO;

public class CustomExternalApi {

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
            System.out.println("[CustomExternalApi] HttpClient created.");

            String endpoint = API_URL;
            HttpPost httpPost = new HttpPost(endpoint);
            httpPost.setHeader(HttpHeaders.AUTHORIZATION, "key=" + API_AUTH_KEY);

            System.out.println("[CustomExternalApi] Building JSON payload...");
            String json = objectMapper.writeValueAsString(new AuthRequestDTO(username, password));
            System.out.println("[CustomExternalApi] Serialized payload: " + json);

            httpPost.setEntity(new StringEntity(json, ContentType.APPLICATION_JSON));
            System.out.println("[CustomExternalApi] Sending request to " + endpoint);

            try (CloseableHttpResponse response = httpClient.execute(httpPost)) {
                System.out.println("[CustomExternalApi] Response received from API.");
                int statusCode = response.getStatusLine().getStatusCode();
                String responseJson = "";
                if (response.getEntity() != null) {
                    responseJson = EntityUtils.toString(response.getEntity(), StandardCharsets.UTF_8);
                }
                System.out.println("[CustomExternalApi] Status code received: " + statusCode);
                System.out.println("[CustomExternalApi] Response body: " + responseJson);

                if (statusCode == 200) {
                    AuthResponseDTO dto = objectMapper.readValue(responseJson, AuthResponseDTO.class);
                    System.out.println("[CustomExternalApi] Authentication successful for user: " + username);
                    return dto;
                } else {
                    System.out.println("[CustomExternalApi] Authentication failed. Status: " + statusCode);
                    return null;
                }
            } catch (Exception e) {
                System.out.println("[CustomExternalApi] Error while executing HTTP request: " + e.getClass().getSimpleName() + " - " + e.getMessage());
                e.printStackTrace(System.out);
                throw e;
            }
        } catch (Exception e) {
            System.out.println("[CustomExternalApi] General error in loginAndGetUserInfo: " + e.getClass().getSimpleName() + " - " + e.getMessage());
            e.printStackTrace(System.out);
            throw e instanceof IOException ? (IOException) e : new IOException(e);
        }
    }
}
