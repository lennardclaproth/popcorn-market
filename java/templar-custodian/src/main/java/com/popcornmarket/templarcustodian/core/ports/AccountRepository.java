package com.popcornmarket.templarcustodian.core.ports;

import com.popcornmarket.templarcustodian.core.domain.entities.Account;

import java.util.Optional;
import java.util.UUID;

public interface AccountRepository {
    Optional<Account> findById(UUID id);
    Optional<Account> findByAccountId(String accountId);
    Account save(Account account);
}
