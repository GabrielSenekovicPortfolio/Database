import { useEffect, useState } from 'react';
import { useNavigate } from "react-router-dom";
import './App.css';

function App() {
    const [skus, setSkus] = useState([]); 
    const [error, setError] = useState("");

    useEffect(() => {
        fetchSkus();
    }, []);

    async function fetchSkus() {
        try {
            const response = await fetch('https://localhost:41262/api/prices/all-skus');

            if (!response.ok) {
                setError(`Failed to fetch SKUs. HTTP Status: ${response.status}`);
                return;
            }

            // Read the response as text first
            const rawText = await response.text();
            console.log("Raw API Response:", rawText); //Debug the raw response

            try {
                // Try parsing as JSON
                const data = JSON.parse(rawText);
                setSkus(data);
            } catch  {
                console.warn("JSON parsing failed. API might be returning HTML.");
                console.error("API returned unexpected response:", rawText);
                setError("Error: API did not return valid JSON. Check console.");
            }
        } catch (error) {
            console.error("Error fetching SKUs:", error);
            setError("Error loading SKUs.");
        }
    }
    const navigate = useNavigate();

    function goToSkuPage(sku) {
        navigate(`/sku/${sku}`);
    }

    return (
        <div className="main-container">
            <h1>Available Products</h1>
            <p>Click on a SKU to see its price history:</p>

            {error && <p style={{ color: 'red' }}>{error}</p>}

            <div className="grid-container">
                {skus.map((sku) => (
                    <div key={sku}>
                        <button onClick={() => goToSkuPage(sku)}>{sku}</button>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default App;
