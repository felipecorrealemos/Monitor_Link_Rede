window.networkCharts = {
    charts: {},
    render: function (canvasId, labels, periodKeys, drops, dotNetRef) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        const existing = this.charts[canvasId];
        if (existing) {
            existing.destroy();
        }

        const safeLabels = (labels && labels.length > 0) ? labels : ['Sem dados'];
        const safeKeys = (periodKeys && periodKeys.length > 0) ? periodKeys : ['none'];
        const safeDrops = (drops && drops.length > 0) ? drops : [0];

        const ctx = canvas.getContext('2d');
        this.charts[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeLabels,
                datasets: [
                    {
                        label: 'Quedas',
                        data: safeDrops,
                        backgroundColor: '#e7a889',
                        borderColor: '#d9916f',
                        borderWidth: 1,
                        borderRadius: 2,
                        barThickness: 16,
                        maxBarThickness: 20
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: false,
                onClick: function (_event, elements) {
                    if (!elements || elements.length === 0 || !dotNetRef) return;
                    const idx = elements[0].index;
                    const key = safeKeys[idx];
                    if (key && key !== 'none') {
                        dotNetRef.invokeMethodAsync('OnChartPointClicked', key);
                    }
                },
                plugins: {
                    legend: { display: false }
                },
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: { color: '#8a8a8a' }
                    },
                    y: {
                        beginAtZero: true,
                        ticks: { precision: 0, color: '#8a8a8a' },
                        grid: { color: '#ececec' },
                        title: { display: true, text: 'Quedas' }
                    }
                }
            }
        });
    }
};
