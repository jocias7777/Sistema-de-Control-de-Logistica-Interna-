// ── Configuración global ──────────────────────────────────────────
const API_BASE = 'https://localhost:7185/api';

// ── Cliente HTTP centralizado ─────────────────────────────────────
async function apiCall(method, endpoint, data = null) {
    const token = sessionStorage.getItem('token');

    const options = {
        method,
        headers: {
            'Content-Type': 'application/json',
            ...(token ? { 'Authorization': `Bearer ${token}` } : {})
        }
    };

    if (data) options.body = JSON.stringify(data);

    showLoading(true);

    try {
        const response = await fetch(`${API_BASE}${endpoint}`, options);
        const json = await response.json();

        if (response.status === 401) {
            auth.logout();
            return;
        }

        return json;
    } catch (error) {
        console.error('Error en apiCall:', error);
        throw error;
    } finally {
        showLoading(false);
    }
}

// ── Manejo de sesión ──────────────────────────────────────────────
const auth = {
    login(data) {
        sessionStorage.setItem('token', data.token);
        sessionStorage.setItem('nombre', data.nombre);
        sessionStorage.setItem('email', data.email);
        sessionStorage.setItem('rol', data.rol);
    },
    logout() {
        sessionStorage.clear();
        window.location.href = '/index.html';
    },
    getUser() {
        return {
            nombre: sessionStorage.getItem('nombre'),
            email: sessionStorage.getItem('email'),
            rol: sessionStorage.getItem('rol')
        };
    },
    isAuthenticated() {
        return !!sessionStorage.getItem('token');
    },
    checkAuth() {
        if (!this.isAuthenticated()) {
            window.location.href = '/index.html';
        }
    },
    hasRole(...roles) {
        const rol = sessionStorage.getItem('rol');
        return roles.includes(rol);
    }
};

// ── Loading spinner ───────────────────────────────────────────────
function showLoading(show) {
    let el = document.getElementById('global-loading');
    if (!el) return;
    el.style.display = show ? 'block' : 'none';
}

// ── Alertas ───────────────────────────────────────────────────────
function showAlert(mensaje, tipo = 'success', containerId = 'alert-container') {
    const el = document.getElementById(containerId);
    if (!el) return;
    el.className = `alert alert-${tipo} show`;
    el.textContent = mensaje;
    setTimeout(() => { el.className = 'alert'; }, 4000);
}

// ── Formato de fechas ─────────────────────────────────────────────
function formatDate(dateStr) {
    if (!dateStr) return '-';
    const d = new Date(dateStr);
    return d.toLocaleDateString('es-GT', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

function formatDateTime(dateStr) {
    if (!dateStr) return '-';
    const d = new Date(dateStr);
    return d.toLocaleString('es-GT');
}

// ── Badge de estado solicitud ─────────────────────────────────────
function badgeEstado(estado) {
    const map = {
        'Borrador': 'badge-borrador',
        'Pendiente': 'badge-pendiente',
        'Aprobada': 'badge-aprobada',
        'Rechazada': 'badge-rechazada',
        'Despachada': 'badge-despachada'
    };
    return `<span class="badge ${map[estado] || ''}">${estado}</span>`;
}

// ── Badge de stock ────────────────────────────────────────────────
function badgeStock(stockActual, stockMinimo) {
    if (stockActual <= 0) return '<span class="badge badge-sin-stock">Sin Stock</span>';
    if (stockActual < stockMinimo) return '<span class="badge badge-bajo">Bajo</span>';
    return '<span class="badge badge-ok">OK</span>';
}

// ── Modal helpers ─────────────────────────────────────────────────
function openModal(id) { document.getElementById(id).classList.add('show'); }
function closeModal(id) { document.getElementById(id).classList.remove('show'); }