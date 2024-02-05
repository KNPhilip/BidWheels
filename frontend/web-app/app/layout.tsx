import type { Metadata } from 'next'
import './globals.css'
import Navbar from './nav/Navbar'
import ToasterProvider from './providers/ToasterProvider'
import SignalRProvider from './providers/SignalRProvider'

export const metadata: Metadata = {
  title: 'BidWheels',
  description: 'BidWheels is a car bidding application, build with Microservices architecture using .NET, NextJS, Duende Identity Server, EF Core, Docker, Kubernetes, RabbitMQ, gRPC, and more....',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body>
        <ToasterProvider />
        <Navbar />
        <main className="container mx-auto px-5 pt-10">
          <SignalRProvider>
            {children}
          </SignalRProvider>
        </main>
      </body>
    </html>
  )
}