CREATE DATABASE account;

\c account;

CREATE TABLE accounts (
    id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    number INTEGER NOT NULL
);