// clearTables.js
// This script drops all tables in the main app database (airy_db) without dropping the database itself.
// Useful for resetting a development environment while keeping the database intact.

import pkg from 'pg';
const { Client } = pkg;

if (!process.env.PGPASSWORD) {
  console.error('❌ Set PGPASSWORD, e.g. PGPASSWORD="$(cat /mnt/hdd/projects/AiryBotCode-data/airy-user.secret)" node scripts/clearDatabase.js (PGHOST, PGPORT, PGUSER, PGDATABASE override the defaults).');
  process.exit(1);
}
const client = new Client({
  host: process.env.PGHOST || '127.0.0.1',
  port: Number(process.env.PGPORT || 5432),
  user: process.env.PGUSER || 'airy_user',
  password: process.env.PGPASSWORD,
  database: process.env.PGDATABASE || 'airy_db'
});

async function clearTables() {
  try {
    console.log('🔌 Connecting to database...');
    await client.connect();
    console.log('✅ Connected!');

    // Drop all tables in the public schema
    console.log('🗑️ Dropping all tables in "airy_db"...');
    await client.query(`
      DO $$ DECLARE
          r RECORD;
      BEGIN
          FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
              EXECUTE 'DROP TABLE IF EXISTS ' || quote_ident(r.tablename) || ' CASCADE';
          END LOOP;
      END $$;
    `);

    console.log('🎉 All tables cleared!');
  } catch (err) {
    console.error('❌ Error clearing tables:', err.message);
  } finally {
    await client.end();
    console.log('🔒 Connection closed.');
  }
}

clearTables();
