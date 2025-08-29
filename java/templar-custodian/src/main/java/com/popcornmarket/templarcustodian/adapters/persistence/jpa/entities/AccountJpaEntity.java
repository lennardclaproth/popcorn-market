package com.popcornmarket.templarcustodian.adapters.persistence.jpa.entities;

import com.popcornmarket.templarcustodian.core.domain.enums.AccountStatus;
import jakarta.persistence.*;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import static com.popcornmarket.templarcustodian.adapters.persistence.jpa.constants.DatabaseConstants.Schemas.ACCOUNT;
import static com.popcornmarket.templarcustodian.adapters.persistence.jpa.constants.DatabaseConstants.Tables.ACCOUNTS;
import static com.popcornmarket.templarcustodian.adapters.persistence.jpa.constants.DatabaseConstants.Tables.ACCOUNT_HOLDINGS;

@Entity
@Table(schema = ACCOUNT, name = ACCOUNTS)
public class AccountJpaEntity {

    @Id
    @Column(name = "id", nullable = false, updatable = false)
    private UUID id;

    @Column(name = "holder", nullable = false)
    private String holder;

    @Column(name = "balance", nullable = false)
    private BigDecimal balance;

    @Enumerated(EnumType.STRING)
    @Column(name = "status", nullable = false)
    private AccountStatus status;

    @ElementCollection
    @CollectionTable(
            name = ACCOUNT_HOLDINGS,
            joinColumns = @JoinColumn(name = "account_id_fk")
    )
    private List<HoldingJpaEmbeddable> holdings = new ArrayList<>();

    public UUID getId(){ return id; }
    public void setId(UUID id){ this.id = id; }
    public String getHolder(){ return holder; }
    public void setHolder(String holder) { this.holder = holder; }
    public BigDecimal getBalance() { return balance; }
    public void setBalance(BigDecimal balance) { this.balance = balance; }
    public AccountStatus getStatus() { return status; }
    public void setStatus(AccountStatus status) { this.status = status; }
    public List<HoldingJpaEmbeddable> getHoldings() { return holdings; }
    public void setHoldings(List<HoldingJpaEmbeddable> holdings) { this.holdings = holdings; }
}
