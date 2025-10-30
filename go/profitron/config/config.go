// internal/config/config.go
package config

import (
	"fmt"
	"os"
	"time"

	"gopkg.in/yaml.v3"
)

const configPath = "config.yaml"

type Config struct {
	Mongo  MongoConfig  `yaml:"mongo"`
	Broker BrokerConfig `yaml:"broker"`
}

type MongoConfig struct {
	URI         string        `yaml:"uri"`
	DBName      string        `yaml:"db_name"`
	AppName     string        `yaml:"app_name"`
	MinPool     uint64        `yaml:"min_pool"`
	MaxPool     uint64        `yaml:"max_pool"`
	ConnTimeout time.Duration `yaml:"conn_timeout"` // parses like "10s", "500ms"
}

type BrokerConfig struct {
	URI string `yaml:"uri"`
}

func ReadConfig() *Config {
	f, err := os.ReadFile(configPath)

	if err != nil {
		fmt.Printf("Error opening config file: %v", err)
		panic(err)
	}

	var cfg Config

	err = yaml.Unmarshal(f, &cfg)

	if err != nil {
		fmt.Printf("Error decoding config: %v", err)
		panic(err)
	}

	return &cfg
}
