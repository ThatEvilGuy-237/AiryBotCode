// clearTables.js
// This script drops all tables in the main app database (airy_db) without dropping the database itself.
// Useful for resetting a development environment while keeping the database intact.

import pkg from 'pg';
const { Client } = pkg;

// PostgreSQL connection config
const client = new Client({
  host: 'localhost',       // Postgres server host
  port: 5432,              // Postgres port
  user: 'airy_user',       // Postgres user
  password: 'Forest_Carrot_93@Night', // Postgres password
  database: 'airy_db'      // Connect directly to your main app database
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
