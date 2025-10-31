package httpx

import (
	"encoding/json"
	"fmt"
	"io"
	"net/http"
)

func DecodeJSONResponse[T any](resp *http.Response) (T, error) {
	var zero T
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return zero, fmt.Errorf("failed to read response body: %w", err)
	}

	if resp.StatusCode < 200 || resp.StatusCode >= 300 {
		var apiErr map[string]string
		if err := json.Unmarshal(body, &apiErr); err == nil && len(apiErr) > 0 {
			return zero, fmt.Errorf("api error (%d): %v", resp.StatusCode, apiErr)
		}
		return zero, fmt.Errorf("unexpected status %d: %s", resp.StatusCode, string(body))
	}

	if len(body) == 0 {
		return zero, nil
	}

	var result T
	if err := json.Unmarshal(body, &result); err != nil {
		return zero, fmt.Errorf("failed to decode JSON: %w", err)
	}
	return result, nil
}
