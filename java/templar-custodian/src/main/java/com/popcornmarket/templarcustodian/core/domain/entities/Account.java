package com.popcornmarket.templarcustodian.core.domain.entities;

import com.popcornmarket.templarcustodian.core.domain.enums.AccountStatus;
import com.popcornmarket.templarcustodian.core.domain.valueobjects.Holding;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class Account {
    private final UUID id;
    private String holder;
    private BigDecimal balance;
    private List<Holding> holdings;
    private AccountStatus status;

    private Account(UUID id, String holder, BigDecimal balance, List<Holding> holdings, AccountStatus status){
        this.id = id;
        this.holder = holder;
        this.balance = balance;
        this.holdings = holdings;
        this.status = status;
    }

    public static Account open(String holder){
        UUID id = UUID.randomUUID();
        BigDecimal balance = BigDecimal.valueOf(0);
        List<Holding> holdings = new ArrayList<>();
        AccountStatus status = AccountStatus.PENDING;

        return new Account(id, holder,balance, holdings, status);
    }

    public static Account hydrate(UUID id, String holder, BigDecimal balance, List<Holding> holdings, AccountStatus status){
        return new Account(id, holder, balance, holdings, status);
    }

    public UUID getId() {
        return id;
    }

    public String getHolder() {
        return holder;
    }

    public BigDecimal getBalance() {
        return balance;
    }

    public List<Holding> getHoldings() {
        return holdings;
    }

    public AccountStatus getStatus() {
        return status;
    }

    public void setStatus(AccountStatus status) {
        this.status = status;
    }
}
