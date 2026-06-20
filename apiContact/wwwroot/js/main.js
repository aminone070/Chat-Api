document.addEventListener('DOMContentLoaded', () => {
  // Highlight active nav link
  const path = window.location.pathname;
  document.querySelectorAll('.nav-links a').forEach(a => {
    if (a.getAttribute('href') === path || (path === '/' && a.getAttribute('href') === '/')) {
      a.classList.add('active');
    }
  });

  // Copy buttons
  document.querySelectorAll('.copy-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      const target = btn.dataset.target;
      const el = document.getElementById(target) || btn.closest('.code-block');
      const text = el ? el.innerText.replace('Copy', '').trim() : '';
      navigator.clipboard.writeText(text).then(() => {
        btn.textContent = 'Copied!';
        setTimeout(() => btn.textContent = 'Copy', 2000);
      });
    });
  });

  // Animate stats counter
  document.querySelectorAll('[data-count]').forEach(el => {
    const target = parseInt(el.dataset.count, 10);
    let current = 0;
    const step = Math.ceil(target / 40);
    const interval = setInterval(() => {
      current = Math.min(current + step, target);
      el.textContent = current + (el.dataset.suffix || '');
      if (current >= target) clearInterval(interval);
    }, 30);
  });

  // Live connection status (polling /health)
  const statusEl = document.getElementById('api-status');
  if (statusEl) {
    const check = async () => {
      try {
        const res = await fetch('/health');
        if (res.ok) {
          statusEl.innerHTML = '<span class="status-dot online"></span> API Online';
          statusEl.className = 'badge badge-green';
        } else throw new Error();
      } catch {
        statusEl.innerHTML = '<span class="status-dot" style="background:#f78166"></span> API Offline';
        statusEl.className = 'badge badge-orange';
      }
    };
    check();
    setInterval(check, 15000);
  }

  // Typewriter effect in hero
  const typeEl = document.getElementById('typewriter');
  if (typeEl) {
    const texts = ['Real-time Messaging', 'WebSocket Events', 'MongoDB Persistence', 'Redis Pub/Sub', 'File Uploads'];
    let i = 0, j = 0, deleting = false;
    const type = () => {
      const txt = texts[i];
      typeEl.textContent = deleting ? txt.slice(0, j--) : txt.slice(0, j++);
      if (!deleting && j > txt.length) { deleting = true; setTimeout(type, 1500); return; }
      if (deleting && j < 0)  { deleting = false; i = (i + 1) % texts.length; j = 0; }
      setTimeout(type, deleting ? 40 : 80);
    };
    type();
  }

  // Fade-in on scroll
  const observer = new IntersectionObserver(entries => {
    entries.forEach(e => { if (e.isIntersecting) { e.target.classList.add('visible'); observer.unobserve(e.target); }});
  }, { threshold: 0.1 });
  document.querySelectorAll('.card, .endpoint-card, .stat').forEach(el => {
    el.style.opacity = '0';
    el.style.transform = 'translateY(16px)';
    el.style.transition = 'opacity .5s ease, transform .5s ease';
    observer.observe(el);
  });
  // callback to apply visible class
  const styleSheet = document.createElement('style');
  styleSheet.textContent = '.visible { opacity: 1 !important; transform: translateY(0) !important; }';
  document.head.appendChild(styleSheet);
});
