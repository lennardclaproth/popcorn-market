package persistence

import (
	"github.com/lennardclaproth/ansar-broker/internal/order"
)

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

func FromDomainOrder(d order.Order) Order {
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
