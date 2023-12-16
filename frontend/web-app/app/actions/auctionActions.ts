'use server'

import { Auction, PagedResult } from "@/types";

export async function getData(pageNumber: number = 1, pageSize: number = 4): Promise<PagedResult<Auction>> {
    const res: Response = await fetch(`http://localhost:6001/search?pageSize=${pageSize}&pageNumber=${pageNumber}`);

    if (res.ok) throw new Error('Failed to fetch data.')

    return res.json();
}