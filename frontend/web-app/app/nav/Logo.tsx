'use client'

import { useParamStore } from '@/hooks/useParamsStore'
import { usePathname, useRouter } from 'next/navigation'
import React from 'react'
import { AiOutlineCar } from 'react-icons/ai'

const Logo = () => {
    const router = useRouter();
    const pathName = usePathname();
    const reset = useParamStore(state => state.reset);

    function doReset() {
        if (pathName !== '/')
            router.push('/');
        reset();
    }

    return (
        <div 
            onClick={doReset}
            className="
                curser-pointer 
                flex 
                items-center 
                gap-2 
                text-3xl 
                font-semibold 
                text-red-500
            "
        >
            <AiOutlineCar size={34} />
            <div>BidWheels Auctions</div>
        </div>
    )
}

export default Logo