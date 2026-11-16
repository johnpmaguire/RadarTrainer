export function attach(scope, dotnet) {
    let pointerId = null;
    let handle = null;
    let latest = null;
    let pending = null;
    let disposed = false;

    function update(event) {
        const matrix = scope.getScreenCTM();
        if (!matrix) {
            console.error('Cannot move the EBL/VRM marker: radar coordinates are unavailable.');
            return;
        }
        const point = new DOMPoint(event.clientX, event.clientY).matrixTransform(matrix.inverse());
        latest = { x: point.x, y: point.y };
        if (!pending) {
            pending = sendUpdates();
        }
    }

    async function sendUpdates() {
        try {
            while (latest && !disposed) {
                const point = latest;
                latest = null;
                await dotnet.invokeMethodAsync('MoveEbl', point.x, point.y);
            }
        } catch (error) {
            console.error('Could not update the EBL/VRM marker.', error);
            latest = null;
        } finally {
            pending = null;
        }
    }

    function pointerDown(event) {
        if (pointerId !== null || event.button !== 0 || !event.target.matches('.ebl-handle')) {
            return;
        }
        event.preventDefault();
        handle = event.target;
        handle.focus();
        pointerId = event.pointerId;
        handle.setPointerCapture(pointerId);
    }

    function pointerMove(event) {
        if (event.pointerId === pointerId) {
            update(event);
        }
    }

    function pointerUp(event) {
        if (event.pointerId === pointerId) {
            pointerId = null;
            handle = null;
        }
    }

    function keyDown(event) {
        if (event.target.matches('.ebl-handle') && ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(event.key)) {
            event.preventDefault();
        }
    }

    scope.addEventListener('pointerdown', pointerDown);
    scope.addEventListener('pointermove', pointerMove);
    scope.addEventListener('pointerup', pointerUp);
    scope.addEventListener('pointercancel', pointerUp);
    scope.addEventListener('lostpointercapture', pointerUp);
    scope.addEventListener('keydown', keyDown);

    return {
        async dispose() {
            disposed = true;
            scope.removeEventListener('pointerdown', pointerDown);
            scope.removeEventListener('pointermove', pointerMove);
            scope.removeEventListener('pointerup', pointerUp);
            scope.removeEventListener('pointercancel', pointerUp);
            scope.removeEventListener('lostpointercapture', pointerUp);
            scope.removeEventListener('keydown', keyDown);
            if (handle && handle.hasPointerCapture(pointerId)) {
                handle.releasePointerCapture(pointerId);
            }
            if (pending) {
                await pending;
            }
        }
    };
}
