// ══════════════════════════════════════════════════════
// utils.js — Funciones globales del sistema
// ══════════════════════════════════════════════════════

const API_BASE = 'http://localhost:5129/api';

// ── CLIENTE HTTP ──────────────────────────────────────
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
    try {
        const response = await fetch(`${API_BASE}${endpoint}`, options);
        if (response.status === 401) { auth.logout(); return null; }
        return await response.json();
    } catch (error) {
        console.error('apiCall error:', error);
        showToast('Error de conexión con el servidor', 'error');
        return null;
    }
}

// ── AUTENTICACIÓN ─────────────────────────────────────
const auth = {
    login(data) {
        sessionStorage.setItem('token', data.token);
        sessionStorage.setItem('nombre', data.nombre);
        sessionStorage.setItem('email', data.email);
        sessionStorage.setItem('rol', data.rol);
        sessionStorage.setItem('usuarioId', data.usuarioId ?? '');
    },
    logout() {
        sessionStorage.clear();
        window.location.href = '/index.html';
    },
    getUser() {
        return {
            nombre: sessionStorage.getItem('nombre'),
            email: sessionStorage.getItem('email'),
            rol: sessionStorage.getItem('rol'),
            usuarioId: sessionStorage.getItem('usuarioId')
        };
    },
    isAuthenticated() { return !!sessionStorage.getItem('token'); },
    checkAuth() {
        if (!this.isAuthenticated()) window.location.href = '/index.html';
    },
    hasRole(...roles) {
        return roles.includes(sessionStorage.getItem('rol'));
    }
};

// ── TOAST NOTIFICATIONS ───────────────────────────────
function showToast(mensaje, tipo = 'success') {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    const icons = {
        success: 'check-circle-fill',
        error: 'x-circle-fill',
        warning: 'exclamation-triangle-fill',
        info: 'info-circle-fill'
    };

    const toast = document.createElement('div');
    toast.className = `toast toast-${tipo}`;
    toast.innerHTML = `<i class="bi bi-${icons[tipo] || icons.info}"></i> ${mensaje}`;
    container.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(20px)';
        toast.style.transition = '.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3500);
}

// Alias para compatibilidad
function showAlert(mensaje, tipo = 'success') {
    showToast(mensaje, tipo === 'danger' ? 'error' : tipo);
}

// ── MODAL HELPERS ─────────────────────────────────────
function openModal(id) { document.getElementById(id).classList.add('show'); }
function closeModal(id) { document.getElementById(id).classList.remove('show'); }

// ── FORMATO FECHAS ────────────────────────────────────
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

// ── BADGES ────────────────────────────────────────────
function badgeEstado(estado) {
    const map = {
        'Borrador': 'badge-borrador',
        'Pendiente': 'badge-pendiente',
        'Aprobada': 'badge-aprobada',
        'Rechazada': 'badge-rechazada',
        'Despachada': 'badge-despachada'
    };
    return `<span class="badge ${map[estado] || 'badge-borrador'}">${estado}</span>`;
}

function badgeStock(stockActual, stockMinimo) {
    if (stockActual <= 0) return '<span class="badge badge-sin-stock">Sin Stock</span>';
    if (stockActual <= stockMinimo) return '<span class="badge badge-bajo">Bajo Mínimo</span>';
    return '<span class="badge badge-ok">OK</span>';
}

function badgeRol(rol) {
    const map = {
        'Admin': 'badge-admin',
        'Bodeguero': 'badge-bodeguero',
        'Solicitante': 'badge-solicitante',
        'Gerente': 'badge-gerente'
    };
    return `<span class="badge ${map[rol] || ''}">${rol}</span>`;
}

function badgeActivo(activo) {
    return activo
        ? '<span class="badge badge-activo">Activo</span>'
        : '<span class="badge badge-inactivo">Inactivo</span>';
}

// ── SIDEBAR CON SUBMENÚS ──────────────────────────────
function initSidebar() {
    const user = auth.getUser();

    // Avatar con inicial
    const avatar = document.getElementById('sidebar-avatar');
    if (avatar) avatar.textContent = (user.nombre || 'U')[0].toUpperCase();

    // Nombre y rol
    const elNombre = document.getElementById('sidebar-nombre');
    const elRol = document.getElementById('sidebar-rol');
    if (elNombre) elNombre.textContent = user.nombre;
    if (elRol) elRol.textContent = user.rol;

    // Submenús — toggle
    document.querySelectorAll('.nav-group-toggle').forEach(toggle => {
        toggle.addEventListener('click', () => {
            const group = toggle.closest('.nav-group');
            const submenu = group.querySelector('.nav-submenu');
            const isOpen = submenu.classList.contains('open');

            // Cerrar todos
            document.querySelectorAll('.nav-submenu').forEach(s => s.classList.remove('open'));
            document.querySelectorAll('.nav-group-toggle').forEach(t => t.classList.remove('open'));

            // Abrir el clickeado si estaba cerrado
            if (!isOpen) {
                submenu.classList.add('open');
                toggle.classList.add('open');
            }
        });
    });

    // Abrir automáticamente el submenú que contiene la página activa
    const currentPath = window.location.pathname;
    document.querySelectorAll('.nav-subitem').forEach(item => {
        if (item.getAttribute('href') === currentPath ||
            item.getAttribute('onclick')?.includes(currentPath)) {
            item.classList.add('active');
            const submenu = item.closest('.nav-submenu');
            const toggle = item.closest('.nav-group')?.querySelector('.nav-group-toggle');
            if (submenu) submenu.classList.add('open');
            if (toggle) toggle.classList.add('open');
        }
    });

    // Resaltar nav-item directo activo
    document.querySelectorAll('.nav-item[href]').forEach(item => {
        if (item.getAttribute('href') === currentPath) {
            item.classList.add('active');
        }
    });

    // Ocultar sección Admin si no es Admin
    const adminSection = document.getElementById('nav-admin-section');
    if (adminSection && !auth.hasRole('Admin')) {
        adminSection.style.display = 'none';
    }

    // Cargar alertas de stock bajo en el topbar
    cargarAlertasStock();
}

// ── ALERTAS STOCK EN TOPBAR ───────────────────────────
async function cargarAlertasStock() {
    const bell = document.getElementById('bell-badge');
    if (!bell) return;
    try {
        const res = await apiCall('GET', '/dashboard/kpis');
        if (res && res.success && res.data.totalProductosBajoMinimo > 0) {
            bell.textContent = res.data.totalProductosBajoMinimo;
            bell.style.display = 'flex';
        } else {
            bell.style.display = 'none';
        }
    } catch (e) { /* silencioso */ }
}

// ── PAGINACIÓN ────────────────────────────────────────
function paginar(data, pagina, porPagina = 15) {
    const inicio = (pagina - 1) * porPagina;
    return {
        items: data.slice(inicio, inicio + porPagina),
        total: data.length,
        totalPaginas: Math.ceil(data.length / porPagina),
        pagina
    };
}

function renderPaginacion(containerId, paginacion, onCambio) {
    const el = document.getElementById(containerId);
    if (!el) return;
    if (paginacion.totalPaginas <= 1) { el.innerHTML = ''; return; }

    let html = `<div class="paginacion">
        <span class="pag-info">${paginacion.total} registros</span>
        <div class="pag-btns">`;

    html += `<button class="pag-btn" ${paginacion.pagina === 1 ? 'disabled' : ''}
                onclick="(${onCambio})(${paginacion.pagina - 1})">
                <i class="bi bi-chevron-left"></i></button>`;

    for (let i = 1; i <= paginacion.totalPaginas; i++) {
        if (i === 1 || i === paginacion.totalPaginas ||
            Math.abs(i - paginacion.pagina) <= 1) {
            html += `<button class="pag-btn ${i === paginacion.pagina ? 'active' : ''}"
                        onclick="(${onCambio})(${i})">${i}</button>`;
        } else if (Math.abs(i - paginacion.pagina) === 2) {
            html += `<span class="pag-dots">…</span>`;
        }
    }

    html += `<button class="pag-btn" ${paginacion.pagina === paginacion.totalPaginas ? 'disabled' : ''}
                onclick="(${onCambio})(${paginacion.pagina + 1})">
                <i class="bi bi-chevron-right"></i></button>`;

    html += '</div></div>';
    el.innerHTML = html;
}

// ── EXPORTAR CSV ──────────────────────────────────────
function exportarCSV(datos, nombreArchivo) {
    if (!datos || datos.length === 0) {
        showToast('No hay datos para exportar', 'warning');
        return;
    }
    const headers = Object.keys(datos[0]);
    const rows = datos.map(d => headers.map(h => `"${d[h] ?? ''}"`).join(','));
    const csv = [headers.join(','), ...rows].join('\n');
    const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = nombreArchivo + '_' + new Date().toISOString().slice(0, 10) + '.csv';
    a.click();
    URL.revokeObjectURL(url);
    showToast('Archivo exportado correctamente', 'success');
}

// ── CONFIRMAR ACCIÓN ──────────────────────────────────
function confirmar(mensaje, onConfirm) {
    if (confirm(mensaje)) onConfirm();
}

// ── LOADING INLINE ────────────────────────────────────
function showLoading(show) {
    let el = document.getElementById('global-loading');
    if (el) el.style.display = show ? 'block' : 'none';
}