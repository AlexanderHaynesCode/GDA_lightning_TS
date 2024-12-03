import {  ReactNode } from 'react'
import NavMenu from './NavMenu'

interface Props {
    children?: ReactNode
}

const Layout = ({ children }: Props) => (
    <div>
        <NavMenu />
        <div id="main">
            {children}
        </div>
        <br />
        <br />
    </div>
)

export default Layout;