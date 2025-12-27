const sql = require('mssql');

const config = {
    user: 'db_9f30c5_mcmewastaging_admin',
    password: 'mcmew@stag1ng',
    server: 'SQL8020.site4now.net',
    database: 'db_9f30c5_mcmewastaging',
    port: 1433,
    options: {
        encrypt: true,
        trustServerCertificate: true
    }
};

async function fetchCompanyData() {
    try {
        console.log('Connecting to MSSQL database...');
        await sql.connect(config);
        console.log('Connected successfully!');

        const result = await sql.query`SELECT * FROM mcmaster.company`;

        console.log('\n=== Company Data ===');
        console.log(`Total records: ${result.recordset.length}`);
        console.log('\nData:');
        console.log(JSON.stringify(result.recordset, null, 2));

        await sql.close();
    } catch (err) {
        console.error('Error:', err.message);
        process.exit(1);
    }
}

fetchCompanyData();
