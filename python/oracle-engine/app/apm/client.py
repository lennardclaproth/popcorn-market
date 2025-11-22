from elasticapm.contrib.starlette import make_apm_client, ElasticAPM

client = make_apm_client({
    'SERVICE_NAME': 'oracle-engine',
    'SERVER_URL': 'http://localhost:8200',
})
