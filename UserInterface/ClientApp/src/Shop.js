import React, { useState } from 'react';
import './Shop.css';
import Dashboard from './Dashboard';
import { useNavigate } from 'react-router-dom';

const Shop = () => {
    const [errorMessage, setErrorMessage] = useState('');
    const [cart, setCart] = useState([]);
    const [availableQuantities, setAvailableQuantities] = useState({
        1: 5,
        2: 5,
        3: 5,
        4: 5,
        5: 5,
    });

    const products = [
        { id: 1, name: 'Moon Star Shoes', price: 19900000 },
        { id: 2, name: 'Passion Diamond Shoe', price: 17000000 },
        { id: 3, name: 'Debbie Wingham Heels', price: 15100000 },
        { id: 4, name: '1998 NBA Finals Game 2 Air Jordan 13s', price: 2200000 },
        { id: 5, name: 'Harry Winston Runy Slippers', price: 3010000 },
    ];

    const navigate = useNavigate();

    const handleCheckout = () => {
        if (cart.length === 0) {
            setErrorMessage('Please fill your cart first.');
        } else {
            // Reset error message
            setErrorMessage('');
            navigate('/delivery', { state: { cart: cart } });
        }
    };

    const addToCart = (product) => {
        if (availableQuantities[product.id] > 0) {
            // Reset error message
            setErrorMessage('');

            const existingItem = cart.find((item) => item.id === product.id);

            if (existingItem) {
                setCart((prevCart) =>
                    prevCart.map((item) =>
                        item.id === product.id ? { ...item, amount: item.amount + 1 } : item
                    )
                );
            } else {
                setCart((prevCart) => [...prevCart, { ...product, amount: 1 }]);
            }

            setAvailableQuantities((prevQuantities) => ({
                ...prevQuantities,
                [product.id]: prevQuantities[product.id] - 1,
            }));
        }
    };

    const removeFromCart = (productId) => {
        const updatedCart = cart
            .map((item) =>
                item.id === productId ? { ...item, amount: item.amount - 1 } : item
            )
            .filter((item) => item.amount > 0);

        setCart(updatedCart);

        setAvailableQuantities((prevQuantities) => ({
            ...prevQuantities,
            [productId]: prevQuantities[productId] + 1,
        }));
    };

    const calculateTotalAmount = () => {
        return cart.reduce((total, item) => total + item.amount * item.price, 0);
    };

    return (
        <div className="Shop">
            <Dashboard />
            <div className="catalog">
                <h2>Product Catalog</h2>
                {products.map((product) => (
                    <div key={product.id} className="list">
                        <div className="card">
                            <p>{product.name} - ${product.price}</p>
                            <p>Amount Left: {availableQuantities[product.id]}</p>
                            <div className='add-button'>
                                <button onClick={() => addToCart(product)}>Add to Cart</button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            <div className="catalog">
                <h2>Shopping Cart</h2>
                {cart.map((item) => (
                    <div key={item.id} className="list">
                        <div className="card">
                            <p>{item.name} - ${item.price}</p>
                            <p>Amount: {item.amount}</p>
                            <div className="remove-button">
                                <button onClick={() => removeFromCart(item.id)}>Remove</button>
                            </div>
                        </div>
                    </div>
                ))}
                <p>Total Amount: ${calculateTotalAmount()}</p>
                <div className='checkout-button'>
                    <button onClick={handleCheckout}>
                        Checkout
                    </button>
                </div>
                {errorMessage && <p className="empty-cart-message" style={{ color: 'red' }}>{errorMessage}</p>}
            </div>
        </div>
    );
};

export default Shop;