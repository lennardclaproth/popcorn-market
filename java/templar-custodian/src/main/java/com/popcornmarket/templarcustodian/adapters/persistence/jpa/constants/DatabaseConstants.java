package com.popcornmarket.templarcustodian.adapters.persistence.jpa.constants;

public final class DatabaseConstants {
    private DatabaseConstants(){ }

    public static final class Schemas {
        public static final String ACCOUNT = "account";
    }

    public static final class Tables {
        public static final String ACCOUNTS = "accounts";
        public static final String ACCOUNT_HOLDINGS = "account_holdings";
    }
}
