import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './Delivery.css';

const Delivery = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const { cart } = location.state || { cart: [] };

    const [address, setAddress] = useState('');
    const [isAddressValid, setIsAddressValid] = useState(true);

    const handleOrder = () => {
        if (address.trim() === '') {
            setIsAddressValid(false);
        } else {
            navigate('/shop');
            console.log('Order placed with address:', address);
        }
    };

    const handleCancel = () => {
        navigate('/shop');
        console.log('Order cancelled');
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
