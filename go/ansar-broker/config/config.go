package config

import (
	"fmt"
	"os"

	"go.yaml.in/yaml/v3"
)

const configPath = "config.yaml"

type Configuration struct {
	Server   Server          `yaml:"server"`
	Database Database        `yaml:"database"`
	Exchange BabylonExchange `yaml:"babylon_exchange"`
	Logging  Logging         `yaml:"logging"`
	APM      APMConfig       `yaml:"apm"`
}

type Logging struct {
	Level int `yaml:"level"`
}

type Server struct {
	Port int `yaml:"port"`
}

type Database struct {
	ConnStr string `yaml:"connection_string"`
	Type    string `yaml:"type"`
}

type BabylonExchange struct {
	URI string `yaml:"uri"`
}

type APMConfig struct {
	ServerURL             string  `yaml:"server_url"`
	ServiceName           string  `yaml:"service_name"`
	Environment           string  `yaml:"environment"`
	SecretToken           string  `yaml:"secret_token"`
	VerifyServerCert      bool    `yaml:"verify_server_cert"`
	LogLevel              string  `yaml:"log_level"`
	TransactionSampleRate float64 `yaml:"transaction_sample_rate"`
}

func ReadConfig() *Configuration {
	f, err := os.ReadFile(configPath)

	if err != nil {
		panic(fmt.Errorf("config: error opening config file at %s: %w", configPath, err))
	}

	var cfg Configuration

	err = yaml.Unmarshal(f, &cfg)

	if err != nil {
		panic(fmt.Errorf("config: error decoding config: %w", err))
	}

	os.Setenv("ELASTIC_APM_SERVER_URL", cfg.APM.ServerURL)
	os.Setenv("ELASTIC_APM_SERVICE_NAME", cfg.APM.ServiceName)
	os.Setenv("ELASTIC_APM_ENVIRONMENT", cfg.APM.Environment)
	os.Setenv("ELASTIC_APM_SECRET_TOKEN", cfg.APM.SecretToken)
	os.Setenv("ELASTIC_APM_VERIFY_SERVER_CERT", fmt.Sprintf("%t", cfg.APM.VerifyServerCert))
	os.Setenv("ELASTIC_APM_LOG_LEVEL", cfg.APM.LogLevel)

	return &cfg
}
