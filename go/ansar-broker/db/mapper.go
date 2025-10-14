package db

import (
	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/internal/order"
	"github.com/lennardclaproth/ansar-broker/internal/security"
	"github.com/lennardclaproth/ansar-broker/internal/user"
)

// Converts a schema model to the domain model
func ToDomainSecurity(s Security) security.Security {
	return security.Security{
		Symbol: s.Symbol,
		Name:   s.Name,
	}
}

// Converts a domain model security to the schema model
func ToSchemaSecurity(d security.Security) Security {
	return Security{
		Symbol: d.Symbol,
		Name:   d.Name,
	}
}

func ToSchemaUser(uu *user.User) User {
	u := User{
		ID:          uu.ID,
		Email:       uu.Email,
		FirstName:   uu.FirstName,
		LastName:    uu.LastName,
		DateOfBirth: uu.DateOfBirth,
		Password:    uu.Password,
		CreatedAt:   uu.CreatedAt,
		Street:      uu.Address.Street,
		City:        uu.Address.City,
		State:       uu.Address.State,
		ZipCode:     uu.Address.ZipCode,
		Country:     uu.Address.Country,
	}

	return u
}

func ToDomainUser(u User) user.User {
	a := user.Address{
		Street:  u.Street,
		City:    u.City,
		State:   u.State,
		ZipCode: u.ZipCode,
		Country: u.Country,
	}

	uu := user.User{
		ID:          u.ID,
		Email:       u.Email,
		FirstName:   u.FirstName,
		LastName:    u.LastName,
		DateOfBirth: u.DateOfBirth,
		Password:    u.Password,
		CreatedAt:   u.CreatedAt,
		Address:     a,
	}

	return uu
}

func ToDomainAccount(a Account) (account.Account, error) {
	portfolio := account.Portfolio{
		TotalValue:    a.TotalValue,
		UnrealizedPnL: a.UnrealizedPnL,
		RealizedPnL:   a.RealizedPnL,
	}

	if a.Holdings != nil {
		portfolio.Holdings = make([]account.Holding, len(a.Holdings))
		for i, h := range a.Holdings {
			portfolio.Holdings[i] = ToDomainHolding(h)
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

func ToSchemaAccount(d account.Account) Account {
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
			acc.Holdings[i] = ToSchemaHolding(h, d.ID)
		}
	}

	return acc
}

func ToSchemaHolding(d account.Holding, accountID uuid.UUID) Holding {
	return Holding{
		Symbol:    d.Security.Symbol,
		Quantity:  d.Quantity,
		AvgPrice:  d.AvgPrice,
		AccountID: accountID,
	}
}

func ToDomainHolding(h Holding) account.Holding {
	return account.Holding{
		Security: ToDomainSecurity(h.Security),
		Quantity: h.Quantity,
		AvgPrice: h.AvgPrice,
	}
}

func ToDomainOrder(o Order) order.Order {
	return order.Order{
		ID:        o.ID,
		AccountID: o.AccountID,
		Ticker:    o.Ticker,
		Quantity:  o.Quantity,
		Price:     o.Price,
		Side:      order.OrderSide(o.Side),
		Type:      order.OrderType(o.Type),
		Status:    order.OrderStatus(o.Status),
	}
}

func ToSchemaOrder(d order.Order) Order {
	return Order{
		ID:        d.ID,
		AccountID: d.AccountID,
		Ticker:    d.Ticker,
		Quantity:  d.Quantity,
		Price:     d.Price,
		Side:      OrderSide(d.Side),
		Type:      OrderType(d.Type),
		Status:    OrderStatus(d.Status),
	}
}
