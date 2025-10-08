package persistence

import (
	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/internal/modules/account"
	security "github.com/lennardclaproth/ansar-broker/internal/security/persistence"
)

func DomainAccount(a Account) (account.Account, error) {
	portfolio := account.Portfolio{
		TotalValue:    a.TotalValue,
		UnrealizedPnL: a.UnrealizedPnL,
		RealizedPnL:   a.RealizedPnL,
	}

	if a.Holdings != nil {
		portfolio.Holdings = make([]account.Holding, len(a.Holdings))
		for i, h := range a.Holdings {
			portfolio.Holdings[i] = DomainHolding(h)
		}
	}

	return account.Account{
		ID:        a.ID,
		UserID:    a.UserID,
		Balance:   a.Balance,
		Status:    account.AccountStatus(a.Status),
		Portfolio: portfolio,
	}, nil
}

func SchemaAccount(d account.Account) Account {
	acc := Account{
		ID:            d.ID,
		UserID:        d.UserID,
		Balance:       d.Balance,
		Status:        AccountStatus(d.Status),
		TotalValue:    d.Portfolio.TotalValue,
		UnrealizedPnL: d.Portfolio.UnrealizedPnL,
		RealizedPnL:   d.Portfolio.RealizedPnL,
	}

	if d.Portfolio.Holdings != nil {
		acc.Holdings = make([]Holding, len(d.Portfolio.Holdings))
		for i, h := range d.Portfolio.Holdings {
			acc.Holdings[i] = SchemaHolding(h, d.ID)
		}
	}

	return acc
}

func SchemaHolding(d account.Holding, accountID uuid.UUID) Holding {
	return Holding{
		Symbol:    d.Security.Symbol,
		Quantity:  d.Quantity,
		AvgPrice:  d.AvgPrice,
		AccountID: accountID,
	}
}

func DomainHolding(h Holding) account.Holding {
	return account.Holding{
		Security: security.DomainSecurity(h.Security),
		Quantity: h.Quantity,
		AvgPrice: h.AvgPrice,
	}
}
