package persistence

import (
	"github.com/lennardclaproth/ansar-broker/internal/security"
)

// Converts a schema model to the domain model
func DomainSecurity(s Security) security.Security {
	return security.Security{
		Symbol: s.Symbol,
		Name:   s.Name,
	}
}

// Converts a domain model security to the schema model
func SchemaSecurity(d security.Security) Security {
	return Security{
		Symbol: d.Symbol,
		Name:   d.Name,
	}
}
