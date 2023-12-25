import EmptyFilter from '@/app/components/EmptyFilter'
import React from 'react'

const Page = ({searchParams}: {searchParams: {callbackUrl: string}}) => {
    return (
        <EmptyFilter 
            title="You need to be logged in to do that."
            subtitle="Please click below to sign in."
            showLogin
            callbackUrl={searchParams.callbackUrl}
        />
    )
}

export default Page;