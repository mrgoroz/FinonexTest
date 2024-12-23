CREATE TABLE IF NOT EXISTS users_revenue (
    user_id VARCHAR(255) PRIMARY KEY,
    revenue INTEGER DEFAULT 0
);

-- Create index for better query performance
CREATE INDEX IF NOT EXISTS idx_user_id ON users_revenue(user_id);
