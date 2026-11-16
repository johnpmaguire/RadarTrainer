export function attach(content) {
    let frame = 0;

    function fit() {
        frame = 0;
        const width = window.innerWidth;
        const height = window.innerHeight;
        content.style.width = `${Math.max(width, 1024)}px`;
        const contentWidth = Math.max(content.offsetWidth, content.scrollWidth);
        const contentHeight = Math.max(content.offsetHeight, content.scrollHeight);
        const scale = Math.min(1, (width - 1) / contentWidth, (height - 1) / contentHeight);
        content.style.transform = `scale(${scale})`;
        document.body.classList.add('viewport-fitted');
    }

    function scheduleFit() {
        if (!frame) {
            frame = requestAnimationFrame(fit);
        }
    }

    const observer = new ResizeObserver(scheduleFit);
    observer.observe(content);
    window.addEventListener('resize', scheduleFit);
    fit();

    return {
        dispose() {
            observer.disconnect();
            window.removeEventListener('resize', scheduleFit);
            if (frame) cancelAnimationFrame(frame);
            content.style.width = '';
            content.style.transform = '';
            document.body.classList.remove('viewport-fitted');
        }
    };
}
