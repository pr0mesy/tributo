const API_URL = 'http://localhost:5022/api/tax/calculate';

const STATES = [
    'AC','AL','AM','AP','BA','CE','DF','ES','GO',
    'MA','MG','MS','MT','PA','PB','PE','PI','PR',
    'RJ','RN','RO','RR','RS','SC','SE','SP','TO'
];

const fmt = (value) =>
    new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);

function populateSelects() {
    const origin = document.getElementById('originState');
    const dest   = document.getElementById('destinationState');

    STATES.forEach(state => {
        origin.add(new Option(state, state));
        dest.add(new Option(state, state));
    });

    origin.value = 'SP';
    dest.value   = 'BA';
}

function showResult(data, productName) {
    const result = document.getElementById('result');
    const error  = document.getElementById('error-msg');

    error.classList.add('hidden');
    result.classList.remove('hidden');

    document.getElementById('result-route').textContent =
        `${data.originState} → ${data.destinationState}`;
    document.getElementById('result-product').textContent = productName;

    document.getElementById('val-icms').textContent = fmt(data.icmsValue);
    document.getElementById('val-iss').textContent  = fmt(data.issValue);
    document.getElementById('val-ipi').textContent  = fmt(data.ipiValue);

    document.getElementById('val-total-tax').textContent  = fmt(data.totalTax);
    document.getElementById('val-final-price').textContent = fmt(data.finalPrice);

    // barras proporcionais ao total de impostos
    const total = data.totalTax || 1;
    setTimeout(() => {
        document.getElementById('bar-icms').style.width = (data.icmsValue / total * 100) + '%';
        document.getElementById('bar-iss').style.width  = (data.issValue  / total * 100) + '%';
        document.getElementById('bar-ipi').style.width  = (data.ipiValue  / total * 100) + '%';
    }, 80);
}

function showError(msg) {
    const result = document.getElementById('result');
    const error  = document.getElementById('error-msg');
    result.classList.add('hidden');
    error.classList.remove('hidden');
    error.textContent = msg;
}

document.getElementById('tax-form').addEventListener('submit', async (e) => {
    e.preventDefault();

    const btn = document.getElementById('calc-btn');
    btn.disabled = true;
    btn.querySelector('.btn-text').textContent = 'Calculando...';

    const productName   = document.getElementById('productName').value.trim();
    const price         = parseFloat(document.getElementById('price').value);
    const type          = parseInt(document.querySelector('input[name="type"]:checked').value);
    const originState   = document.getElementById('originState').value;
    const destinationState = document.getElementById('destinationState').value;

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productName, price, type, originState, destinationState })
        });

        if (!response.ok) {
            const err = await response.text();
            throw new Error(err || `Erro ${response.status}`);
        }

        const data = await response.json();
        showResult(data, productName);

    } catch (err) {
        if (err.message.includes('fetch')) {
            showError('Não foi possível conectar à API. Verifique se o servidor está rodando.');
        } else {
            showError('Erro ao calcular impostos: ' + err.message);
        }
    } finally {
        btn.disabled = false;
        btn.querySelector('.btn-text').textContent = 'Calcular impostos';
    }
});

populateSelects();