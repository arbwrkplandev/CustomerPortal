window.wrkplanPayments = window.wrkplanPayments || {};

window.wrkplanPayments.openRazorpayCheckout = async function (options) {
    const ensureScript = () => new Promise((resolve, reject) => {
        if (window.Razorpay) {
            resolve();
            return;
        }

        const script = document.createElement('script');
        script.src = 'https://checkout.razorpay.com/v1/checkout.js';
        script.async = true;
        script.onload = () => resolve();
        script.onerror = () => reject(new Error('Unable to load Razorpay checkout script.'));
        document.body.appendChild(script);
    });

    await ensureScript();

    return await new Promise((resolve, reject) => {
        const rz = new Razorpay({
            key: options.key,
            amount: options.amount,
            currency: options.currency,
            name: options.name,
            description: options.description,
            order_id: options.orderId,
            prefill: {
                name: options.customerName,
                email: options.customerEmail
            },
            notes: {
                invoiceId: options.invoiceId,
                invoiceNumber: options.invoiceNumber
            },
            theme: {
                color: '#0f766e'
            },
            handler: function (response) {
                resolve(response);
            },
            modal: {
                ondismiss: function () {
                    reject(new Error('Payment cancelled by user.'));
                }
            }
        });

        rz.open();
    });
};
