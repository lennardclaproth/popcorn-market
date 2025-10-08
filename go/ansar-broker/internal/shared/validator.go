package shared

import (
	"time"

	"github.com/go-playground/validator/v10"
)

func DateOnlyValidator(fl validator.FieldLevel) bool {
	value := fl.Field().String() // Get the field as a string

    if value == "" {
        return false
    }

    _, err := time.Parse(time.DateOnly, value)
    return err == nil
}

func NewValidator() *validator.Validate {
	v := validator.New()
	v.RegisterValidation("dateonly", DateOnlyValidator)

	return v
}