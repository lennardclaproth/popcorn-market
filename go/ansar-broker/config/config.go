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

func ReadConfig() *Configuration {
	f, err := os.ReadFile(configPath)

	if err != nil {
		fmt.Printf("Error opening config file: %v", err)
		panic(err)
	}

	var cfg Configuration

	err = yaml.Unmarshal(f, &cfg)

	if err != nil {
		fmt.Printf("Error decoding config: %v", err)
		panic(err)
	}

	return &cfg
}
