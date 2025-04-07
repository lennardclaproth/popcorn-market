# logging_config.py
import logging.config

LOGGING_CONFIG = {
    "version": 1,
    "disable_existing_loggers": False,
    "formatters": {
        "default": {
            "format": "[%(asctime)s] %(levelname)s - %(name)s - %(message)s",
        },
    },
    "handlers": {
        "console": {
            "class": "logging.StreamHandler",
            "formatter": "default",
        },
    },
    "loggers": {
        "worker_app": {
            "handlers": ["console"],
            "level": "INFO",
            "propagate": False
        },
        "uvicorn.error": {  # capture uvicorn internal logs
            "handlers": ["console"],
            "level": "INFO",
            "propagate": False
        },
        "uvicorn.access": {  # capture uvicorn access logs
            "handlers": ["console"],
            "level": "INFO",
            "propagate": False
        }
    },
    "root": {  # fallback logger if no logger is explicitly defined
        "level": "WARNING",
        "handlers": ["console"]
    }
}
