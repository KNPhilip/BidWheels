'use client'

import { Pagination } from 'flowbite-react'
import React, { useState } from 'react'

type Props = {
    currentPage: number,
    pageCount: number
}

const AppPagination = ({currentPage, pageCount}: Props) => {
    const [pageNumber, setPageNumber] = useState(currentPage);

    return (
        <Pagination 
            currentPage={pageNumber}
            onPageChange={e => setPageNumber(e)}
            totalPages={pageCount}
            layout="pagination"
            showIcons={true}
            className="text-blue-500 mb-5"
        />
    )
}

export default AppPagination;