package com.popcornmarket.templarcustodian.adapters.persistence.jpa.mapper;

import com.popcornmarket.templarcustodian.adapters.persistence.jpa.entities.AccountJpaEntity;
import com.popcornmarket.templarcustodian.core.domain.entities.Account;

public class AccountJpaMapper {

    private AccountJpaMapper() { }

    public static Account toDomain(AccountJpaEntity e){
        if (e == null) return null;

        return Account.hydrate(
                e.getId(),
                e.getHolder(),
                e.getBalance(),
                HoldingJpaEmbeddableMapper.toDomainList(e.getHoldings()),
                e.getStatus()
        );
    }

    public static AccountJpaEntity toEntity(Account d){
        if (d == null) return null;

        var e = new AccountJpaEntity();
        e.setId(d.getId());
        e.setHolder(d.getHolder());
        e.setBalance(d.getBalance());
        e.setStatus(d.getStatus());
        e.setHoldings(HoldingJpaEmbeddableMapper.toEmbeddableList(d.getHoldings()));
        return e;
    }
}
