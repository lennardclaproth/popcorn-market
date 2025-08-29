package com.popcornmarket.templarcustodian.adapters.persistence.jpa.entities;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import jakarta.persistence.Table;

import java.math.BigInteger;

import static com.popcornmarket.templarcustodian.adapters.persistence.jpa.constants.DatabaseConstants.Schemas.ACCOUNT;
import static com.popcornmarket.templarcustodian.adapters.persistence.jpa.constants.DatabaseConstants.Tables.ACCOUNT_HOLDINGS;

@Embeddable
@Table(schema = ACCOUNT, name=ACCOUNT_HOLDINGS)
public class HoldingJpaEmbeddable {
    @Column(name = "ticker", nullable = false)
    private String ticker;

    @Column(name = "quantity", nullable = false)
    private BigInteger quantity;

    public String getTicker() {
        return ticker;
    }

    public void setTicker(String ticker) {
        this.ticker = ticker;
    }

    public BigInteger getQuantity() {
        return quantity;
    }

    public void setQuantity(BigInteger quantity) {
        this.quantity = quantity;
    }
}
