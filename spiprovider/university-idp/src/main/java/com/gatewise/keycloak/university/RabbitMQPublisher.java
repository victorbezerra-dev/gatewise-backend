package com.gatewise.keycloak.university;

import java.nio.charset.StandardCharsets;

import org.jboss.logging.Logger;

import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;
import com.rabbitmq.client.MessageProperties;

public class RabbitMQPublisher {

    private static final Logger LOG = Logger.getLogger(RabbitMQPublisher.class);

    private static final String DEFAULT_HOST = "rabbitmq";
    private static final int DEFAULT_PORT = 5672;
    private static final String DEFAULT_QUEUE_NAME = "user.login.sync";

    public boolean tryPublish(String message) {
        ConnectionFactory factory = new ConnectionFactory();

        String host = getEnvOrDefault("RABBITMQ_HOST", DEFAULT_HOST);
        int port = getIntEnvOrDefault("RABBITMQ_PORT", DEFAULT_PORT);
        String username = getRequiredEnv("RABBITMQ_USER");
        String password = getRequiredEnv("RABBITMQ_PASS");
        String queueName = getEnvOrDefault("RABBITMQ_QUEUE", DEFAULT_QUEUE_NAME);

        factory.setHost(host);
        factory.setPort(port);
        factory.setUsername(username);
        factory.setPassword(password);
        factory.setAutomaticRecoveryEnabled(true);

        try (Connection connection = factory.newConnection(); Channel channel = connection.createChannel()) {

            channel.confirmSelect();
            channel.queueDeclare(queueName, true, false, false, null);

            channel.basicPublish("", queueName, MessageProperties.PERSISTENT_TEXT_PLAIN, message.getBytes(StandardCharsets.UTF_8));
            channel.waitForConfirmsOrDie(5000);

            LOG.infof("[RabbitMQPublisher] Message published successfully. host=%s port=%d queue=%s username=%s", host, port, queueName, username);

            return true;

        } catch (Exception e) {
            LOG.errorf(e, "[RabbitMQPublisher] Error publishing to RabbitMQ. host=%s port=%d queue=%s username=%s", host, port, queueName, username);
            return false;
        }
    }

    private static String getRequiredEnv(String name) {
        String value = System.getenv(name);
        if (value == null || value.trim().isEmpty()) {
            throw new IllegalStateException("Environment variable " + name + " is required for RabbitMQPublisher.");
        }
        return value.trim();
    }

    private static String getEnvOrDefault(String name, String defaultValue) {
        String value = System.getenv(name);
        return value == null || value.trim().isEmpty() ? defaultValue : value.trim();
    }

    private static int getIntEnvOrDefault(String name, int defaultValue) {
        String value = System.getenv(name);
        if (value == null || value.trim().isEmpty()) {
            return defaultValue;
        }

        try {
            return Integer.parseInt(value.trim());
        } catch (NumberFormatException e) {
            throw new IllegalStateException("Environment variable " + name + " must be a valid integer.", e);
        }
    }

}
