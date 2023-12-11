﻿import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './Delivery.css';

const Delivery = () => {
    const [highestDeliveryId, setHighestDeliveryId] = useState(0);

    useEffect(() => {
        const fetchAllDeliveries = async () => {
            try {
                const response = await fetch('http://localhost:5062/api/deliveries');
                if (response.ok) {
                    const data = await response.json();
                    if (data.length > 0) {
                        const maxId = Math.max(...data.map(delivery => delivery.id));
                        setHighestDeliveryId(maxId);
                    } else {
                        // If there are no deliveries, set the highestDeliveryId to 0 or 1, depending on your server's expectations
                        setHighestDeliveryId(1);
                    }
                } else {
                    console.error('Failed to fetch all deliveries:', response.statusText);
                }
            } catch (error) {
                console.error('Failed to fetch all deliveries:', error.message);
            }
        };

        fetchAllDeliveries();
    }, []); // Fetch all deliveries once when the component mounts
    const location = useLocation();
    const navigate = useNavigate();
    const { cart } = location.state || { cart: [] };

    const [address, setAddress] = useState('');
    const [isAddressValid, setIsAddressValid] = useState(true);

    const handleOrder = () => {
        if (address.trim() === '') {
            setIsAddressValid(false);
        } else {
            const userId = '1';
            const paymentId = '1';
            const geoX = Math.random();
            const geoY = Math.random();

            // Current time for deliveryTime
            const deliveryTime = new Date().toISOString();

            // Make a request to add a delivery
            fetch('http://localhost:5062/api/deliveries', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    //'Authorization': `Bearer ${yourStoredJWT}`, // Replace with the actual JWT token
                },
                body: JSON.stringify({
                    id: (highestDeliveryId + 1).toString(),
                    userId,
                    paymentId,
                    address,
                    deliveryTime,
                    geoX,
                    geoY,
                }),
            })
                .then(response => {
                    if (response.ok) {
                        console.log('Order placed successfully');
                        navigate('/shop');
                    } else {
                        console.error('Failed to add delivery');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    };


    const handleCancel = () => {
        navigate('/shop');
        console.log('Order cancelled');
    };

    const calculateTotalAmount = () => {
        return cart.reduce((total, item) => total + item.amount * item.price, 0);
    };

    return (
        <div className="Delivery">
            <h2>Delivery Page</h2>
            {cart.length > 0 ? (
                <div className="cart-container">
                    <h3>Your Cart:</h3>
                    {cart.map((item) => (
                        <div key={item.id} className="cart-item">
                            <p>
                                {item.name} - ${item.price} - Amount: {item.amount}
                            </p>
                        </div>
                    ))}
                    <p>Total Amount: ${calculateTotalAmount()}</p>
                    <div className="address-input">
                        <p>Address:</p>
                        <input
                            type="text"
                            value={address}
                            onChange={(e) => {
                                setAddress(e.target.value);
                                setIsAddressValid(true);
                            }}
                        />
                    </div>
                    {!isAddressValid && <p className="validation-message">Please enter a non-empty address.</p>}
                    <div className="button-container">
                        <div className="cancel-button">
                            <button onClick={handleCancel}>Cancel</button>
                        </div>
                        <div className="order-button">
                            <button onClick={handleOrder} disabled={!isAddressValid}>
                                Order
                            </button>
                        </div>
                    </div>
                </div>
            ) : (
                <p>Your cart is empty. Add items to proceed to checkout.</p>
            )}
        </div>
    );
};

export default Delivery;
