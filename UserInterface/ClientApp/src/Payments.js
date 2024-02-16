import React, { useState, useEffect } from 'react';
import './Payments.css';
import Dashboard from './Dashboard';

const PaymentHistory = () => {
    const [payments, setPayments] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const userId = localStorage.getItem('userId');

    useEffect(() => {
        const fetchPayments = async () => {
            try {
                const response = await fetch(`https://localhost:11183/CartPayment/payments/${userId}`, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch payments');
                }

                const data = await response.json();
                setPayments(data);
            } catch (err) {
                setError(err.message);
            } finally {
                setIsLoading(false);
            }
        };

        if (userId) {
            fetchPayments();
        }
    }, [userId]);

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error}</div>;
    }

    return (
        <div className="Payment">
            <Dashboard />
            <div className="payment-container">
                <h2>Payment History</h2>
                {payments.length > 0 ? (
                    <ul>
                        {payments.map(payment => (
                            <li key={payment.id} className="paymentItem">
                                <p>Payment ID: {payment.id}</p>
                                <p>Amount: ${payment.amount}</p>
                                <p>Date: {new Date(payment.paymentDate).toLocaleDateString()}</p>
                                {/* Add more payment details as needed */}
                            </li>
                        ))}
                    </ul>
                ) : (
                    <p>No payments found.</p>
                )}
            </div>
        </div>
    );
};

export default PaymentHistory;
