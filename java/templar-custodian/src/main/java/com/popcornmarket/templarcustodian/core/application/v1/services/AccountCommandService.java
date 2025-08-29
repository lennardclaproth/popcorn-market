package com.popcornmarket.templarcustodian.core.application.v1.services;

import com.popcornmarket.templarcustodian.core.application.v1.commands.OpenAccountCommand;
import com.popcornmarket.templarcustodian.core.domain.entities.Account;
import com.popcornmarket.templarcustodian.core.ports.AccountRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.UUID;

@Service
public class AccountCommandService {

    private final AccountRepository accountRepository;

    public AccountCommandService(AccountRepository accountRepository) {
        this.accountRepository = accountRepository;
    }

    @Transactional
    public UUID openAccount(OpenAccountCommand command){
        Account account = Account.open(command.holder());

        accountRepository.save(account);
        return account.getId();
    }
}
