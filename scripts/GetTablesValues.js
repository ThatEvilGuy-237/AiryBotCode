// GetTablesValues.js
// This script connects to the PostgreSQL database and fetches all records from all tables,
// printing them in a well-formatted manner.

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

async function getTablesValues() {
  try {
    console.log('🔌 Connecting to database...');
    await client.connect();
    console.log('✅ Connected!');

    // Fetch all table names
    const tablesResult = await client.query(`
      SELECT tablename
      FROM pg_tables
      WHERE schemaname = 'public';
    `);
    const tableNames = tablesResult.rows.map(row => row.tablename);

    if (tableNames.length === 0) {
      console.log('No tables found in the public schema.');
      return;
    }

    console.log('\n--- Fetching Data from Tables ---');

    for (const tableName of tableNames) {
      console.log(`\n### Table: ${tableName} ###`);
      try {
        const dataResult = await client.query(`SELECT * FROM "${tableName}"`);
        if (dataResult.rows.length === 0) {
          console.log('  (No records)');
        } else {
          dataResult.rows.forEach(row => {
            console.log(JSON.stringify(row, null, 2)); // Pretty print each row
          });
        }
      } catch (tableErr) {
        console.error(`  ❌ Error fetching data from table '${tableName}': ${tableErr.message}`);
      }
    }

    console.log('\n--- Data Fetching Complete ---');

  } catch (err) {
    console.error('❌ Error during database operation:', err.message);
  } finally {
    await client.end();
    console.log('🔒 Connection closed.');
  }
}

getTablesValues();
