package com.gatewise.keycloak.university.utils;

import javax.sql.DataSource;

import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;

public class DataSourceProvider {

    private static final HikariDataSource dataSource;

    static {
        String dbHost = getEnvOrDefault("GATEWISE_DB_HOST", "gatewise-db");
        String dbPort = getEnvOrDefault("GATEWISE_DB_PORT", "5432");
        String dbName = getRequiredEnv("GATEWISE_DB_NAME");
        String dbUser = getRequiredEnv("GATEWISE_DB_USER");
        String dbPassword = getRequiredEnv("GATEWISE_DB_PASSWORD");
        String jdbcUrl = String.format("jdbc:postgresql://%s:%s/%s", dbHost, dbPort, dbName);

        HikariConfig config = new HikariConfig();
        config.setJdbcUrl(jdbcUrl);
        config.setUsername(dbUser);
        config.setPassword(dbPassword);
        config.setMaximumPoolSize(10);
        config.setConnectionTimeout(3000);
        config.setIdleTimeout(60000);
        config.setMaxLifetime(1800000);
        dataSource = new HikariDataSource(config);
    }

    public static DataSource get() {
        return dataSource;
    }

    private static String getRequiredEnv(String name) {
        String value = System.getenv(name);
        if (value == null || value.trim().isEmpty()) {
            throw new IllegalStateException("Environment variable " + name + " is required for DataSourceProvider.");
        }
        return value.trim();
    }

    private static String getEnvOrDefault(String name, String defaultValue) {
        String value = System.getenv(name);
        return value == null || value.trim().isEmpty() ? defaultValue : value.trim();
    }
}
