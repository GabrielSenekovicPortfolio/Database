import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';

function SkuPage() {
    const { sku } = useParams();
    const navigate = useNavigate();
    const [prices, setPrices] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        fetchPriceData();
    });

    async function fetchPriceData() {
        try {
            const response = await fetch(`https://localhost:41262/api/prices/${sku}`);
            if (response.ok) {
                const data = await response.json();
                setPrices(data);
            } else {
                setError("No prices found for this SKU.");
            }
        } catch (error) {
            console.error("Error fetching prices:", error);
            setError("Error fetching price data.");
        }
    }

    return (
        <div>
            <h1>Price History for {sku}</h1>
            {error && <p style={{ color: 'red' }}>{error}</p>}

            {prices.length === 0 ? (
                <p><em>Loading or no data available...</em></p>
            ) : (
                <table>
                    <thead>
                        <tr>
                            <th>Market</th>
                            <th>Currency</th>
                            <th>Price</th>
                            <th>Valid From</th>
                            <th>Valid Until</th>
                        </tr>
                    </thead>
                    <tbody>
                        {prices.map((price, index) => (
                            <tr key={index}>
                                <td>{price.marketId}</td>
                                <td>{price.currencyCode}</td>
                                <td>{price.unitPrice}</td>
                                <td>{price.validFrom}</td>
                                <td>{price.validUntil ?? "Always Valid"}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
            <button onClick={() => navigate("/")}>Back to All SKUs</button>
        </div>
    );
}

export default SkuPage;
