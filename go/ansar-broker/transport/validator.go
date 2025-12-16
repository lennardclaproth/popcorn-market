package transport

import (
	"context"
	"time"
)

// Validator is an object that can be validated.
type Validator interface {
	// Valid checks the object and returns any
	// problems. If len(problems) == 0 then
	// the object is valid.
	Valid(ctx context.Context) (problems map[string]string)
}

func ValidateDateOnly(d string) bool {
	if d == "" {
		return false
	}

	_, err := time.Parse(time.DateOnly, d)
	return err == nil
}
