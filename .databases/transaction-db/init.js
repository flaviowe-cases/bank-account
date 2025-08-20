db.createUser(
        {
            user: "admin",
            pwd: "admin",
            roles: [
                {
                    role: "dbAdmin",
                    db: "transaction-db"
                }
            ]
        }
);