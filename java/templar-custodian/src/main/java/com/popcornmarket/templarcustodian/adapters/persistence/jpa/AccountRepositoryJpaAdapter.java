package com.popcornmarket.templarcustodian.adapters.persistence.jpa;

import com.popcornmarket.templarcustodian.adapters.persistence.jpa.mapper.AccountJpaMapper;
import com.popcornmarket.templarcustodian.adapters.persistence.jpa.repositories.AccountJpaRepository;
import com.popcornmarket.templarcustodian.core.domain.entities.Account;
import com.popcornmarket.templarcustodian.core.ports.AccountRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;
import java.util.UUID;

@Repository
public class AccountRepositoryJpaAdapter implements AccountRepository {

    private final AccountJpaRepository accountJpaRepository;

    public AccountRepositoryJpaAdapter(AccountJpaRepository accountJpaRepository) {
        this.accountJpaRepository = accountJpaRepository;
    }

    @Override
    public Optional<Account> findById(UUID id) {
        return accountJpaRepository.findById(id).map(AccountJpaMapper::toDomain);
    }

    @Override
    public Optional<Account> findByAccountId(String accountId) {
        return Optional.empty();
    }

    @Override
    public Account save(Account account) {
        return null;
    }
}
