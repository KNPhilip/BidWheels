'use server'

import { Auction, PagedResult } from "@/types";
import { getTokenWorkaround } from "./authActions";
import { fetchWrapper } from "@/lib/fetchWrapper";

export async function getData(query: string): Promise<PagedResult<Auction>> {
    return await fetchWrapper.get(`search${query}`);
}

export async function UpdateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 100000) + 1,
    }

    const token = await getTokenWorkaround();

    const res = await fetch('http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', {
        method: 'PUT',
        headers: {
            'Content-type': 'application/json',
            'Authorization': 'Bearer ' + token?.access_token
        },
        body: JSON.stringify(data)
    });

    if (res.ok) return {status: res.status, message: res.statusText}

    return res.statusText;
}