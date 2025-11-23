-- Clean up alerts and reset coin activation state
--
-- Usage:
--   For Alert Service database (alerts table):
--   psql -h localhost -U postgres -d alertservice -f cleanup-alerts-and-coins.sql
--
--   For CienceTerminal database (coins table):
--   psql -h localhost -U postgres -d cienceterminal -f cleanup-alerts-and-coins.sql

-- =============================================================================
-- ALERT SERVICE DATABASE
-- =============================================================================
-- Delete all alerts from Alert Service database
TRUNCATE TABLE alerts;

-- =============================================================================
-- CIENCETERMINAL DATABASE
-- =============================================================================
-- Reset all coins to IsActive = false
UPDATE coins SET "IsActive" = false, "LastUpdated" = NOW();

-- =============================================================================
-- VERIFICATION
-- =============================================================================
-- Verify cleanup
SELECT 'Alerts remaining:' as info, COUNT(*) as count FROM alerts
UNION ALL
SELECT 'Active coins:' as info, COUNT(*) as count FROM coins WHERE "IsActive" = true
UNION ALL
SELECT 'Total coins:' as info, COUNT(*) as count FROM coins;
