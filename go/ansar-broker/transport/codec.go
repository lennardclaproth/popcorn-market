package transport

import (
	"encoding/json"
	"fmt"
	"net/http"
	"reflect"
	"strconv"
)

func JSONDecoder[T any](r *http.Request) (T, error) {
	var req T
	err := json.NewDecoder(r.Body).Decode(&req)
	return req, err
}

func QueryDecoder[T any](r *http.Request) (T, error) {
	var target T
	values := r.URL.Query()

	v := reflect.ValueOf(&target).Elem()
	t := v.Type()

	for i := 0; i < t.NumField(); i++ {
		field := t.Field(i)
		tag := field.Tag.Get("query")
		if tag == "" {
			continue
		}

		val := values.Get(tag)
		if val == "" {
			continue // optional param
		}

		f := v.Field(i)
		if !f.CanSet() {
			continue
		}

		switch f.Kind() {
		case reflect.String:
			f.SetString(val)
		case reflect.Int, reflect.Int64:
			i, err := strconv.Atoi(val)
			if err != nil {
				return target, fmt.Errorf("invalid int for %s: %w", tag, err)
			}
			f.SetInt(int64(i))
		case reflect.Float64:
			fv, err := strconv.ParseFloat(val, 64)
			if err != nil {
				return target, fmt.Errorf("invalid float for %s: %w", tag, err)
			}
			f.SetFloat(fv)
		case reflect.Bool:
			bv, err := strconv.ParseBool(val)
			if err != nil {
				return target, fmt.Errorf("invalid bool for %s: %w", tag, err)
			}
			f.SetBool(bv)
		default:
			// silently ignore unsupported types
		}
	}
	return target, nil
}

func encode[T any](w http.ResponseWriter, status int, v T) error {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	if err := json.NewEncoder(w).Encode(v); err != nil {
		return fmt.Errorf("encode json: %w", err)
	}
	return nil
}
