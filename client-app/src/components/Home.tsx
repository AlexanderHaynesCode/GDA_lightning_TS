import { Link } from 'react-router-dom';

const Home = () => (
    <div>
        <h1 className='Home_GDA_title'>Generic Dating App</h1>
        <div className='text-center d11'>
          <h2 className='homeSubTitle'>The one-size-fits-all dating app!</h2>  
          <Link to="/SignUp" className='p1 SignUpHomeBtn' id='SignUpHomeBtn'>Sign-Up</Link>
        </div>
      </div>
)

export default Home;