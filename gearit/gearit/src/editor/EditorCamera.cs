using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor
{
	class EditorCamera : ICamera
	{
		private const float _minZoom = 0.02f;
		private const float _maxZoom = 20f;
		private static GraphicsDevice _graphics;

		private Matrix _batchView;

		private Vector2 _currentPosition;

		private float _currentRotation;

		private float _currentZoom;
		private Vector2 _maxPosition;
		private float _maxRotation;
		private Vector2 _minPosition;
		private float _minRotation;
		private Matrix _projection;
		private Vector2 _translateCenter;
		private Matrix _view;

		/// <summary>
		/// The constructor for the Camera2D class.
		/// </summary>
		/// <param name="graphics"></param>
		public EditorCamera(GraphicsDevice graphics)
		{
			_minPosition = new Vector2(-1000f, -1000f);
			_maxPosition = new Vector2(1000f, 1000f);

			_graphics = graphics;
			_projection = Matrix.CreateOrthographicOffCenter(0f, ConvertUnits.ToSimUnits(_graphics.Viewport.Width),
															 ConvertUnits.ToSimUnits(_graphics.Viewport.Height), 0f, 0f,
															 1f);
			_view = Matrix.Identity;
			_batchView = Matrix.Identity;

			_translateCenter = new Vector2(ConvertUnits.ToSimUnits(_graphics.Viewport.Width / 2f),
										   ConvertUnits.ToSimUnits(_graphics.Viewport.Height / 2f));

			reset();
		}


		public Matrix View
		{
			get { return _batchView; }
		}

		public Matrix view()
	{
			return _view;
		}

		public Matrix projection()
		{
			return _projection;
		}

		/// <summary>
		/// The current position of the camera.
		/// </summary>
		public Vector2 Position
		{
			get { return _currentPosition; }
			set
			{
				_currentPosition = ConvertUnits.ToSimUnits(value);
				if (_minPosition != _maxPosition)
				{
					Vector2.Clamp(ref _currentPosition, ref _minPosition, ref _maxPosition, out _currentPosition);
				}
			}
		}

		/// <summary>
		/// The current rotation of the camera in radians.
		/// </summary>
		public float Rotation
		{
			get { return _currentRotation; }
			set
			{
				_currentRotation = value % MathHelper.TwoPi;
				if (_minRotation != _maxRotation)
				{
					_currentRotation = MathHelper.Clamp(_currentRotation, _minRotation, _maxRotation);
				}
			}
		}

		/// <summary>
		/// The current zoom
		/// </summary>
		public void zoomIn()
		{
			Zoom *= 1.1f;
		}

		public void zoomOut()
		{
			Zoom /= 1.1f;
		}

		public float Zoom
		{
			get { return _currentZoom; }
			set
			{
				_currentZoom = value;
				_currentZoom = MathHelper.Clamp(_currentZoom, _minZoom, _maxZoom);
			}
		}

		public void move(Vector2 amount)
		{
			_currentPosition -= ConvertUnits.ToSimUnits(amount / Zoom);
			if (_minPosition != _maxPosition)
			{
				Vector2.Clamp(ref _currentPosition, ref _minPosition, ref _maxPosition, out _currentPosition);
			}
		}

		public void rotate(float amount)
		{
			_currentRotation += amount;
			if (_minRotation != _maxRotation)
			{
				_currentRotation = MathHelper.Clamp(_currentRotation, _minRotation, _maxRotation);
			}
		}

		/// <summary>
		/// Resets the camera to default values.
		/// </summary>
		public void reset()
		{
			_currentPosition = Vector2.Zero;

			_currentRotation = 0f;
			_minRotation = -MathHelper.Pi;
			_maxRotation = MathHelper.Pi;

			_currentZoom = 1f;

			update();
		}

		public void update()
		{

			Matrix matRotation = Matrix.CreateRotationZ(_currentRotation);
			Matrix matZoom = Matrix.CreateScale(_currentZoom);
			Vector3 translateCenter = new Vector3(_translateCenter, 0f);
			Vector3 translateBody = new Vector3(-_currentPosition, 0f);

			_view = Matrix.CreateTranslation(translateBody - translateCenter) * matZoom *
			Matrix.CreateTranslation(translateCenter);

			translateCenter = ConvertUnits.ToDisplayUnits(translateCenter);
			translateBody = ConvertUnits.ToDisplayUnits(translateBody);

			_batchView = Matrix.CreateTranslation(translateBody) *
						 matRotation *
						 matZoom *
						 Matrix.CreateTranslation(translateCenter);
			Input.SimMousePos = (ConvertUnits.ToSimUnits(Input.position()) / Zoom + Position + (center() - center() / Zoom));
		}

		public Vector2 center()
		{
			return (_translateCenter);
		}

		public Vector2 ConvertScreenToWorld(Vector2 location)
		{
			Vector3 t = new Vector3(location, 0);

			t = _graphics.Viewport.Unproject(t, _projection, _view, Matrix.Identity);

			return new Vector2(t.X, t.Y);
		}

		public Vector2 ConvertWorldToScreen(Vector2 location)
		{
			Vector3 t = new Vector3(location, 0);

			t = _graphics.Viewport.Project(t, _projection, _view, Matrix.Identity);

			return new Vector2(t.X, t.Y);
		}

		public void input(bool canMove = true)
		{
			if (canMove && Input.pressed(MouseKeys.MIDDLE) || (Input.pressed(Keys.V)))
				move(Input.mouseOffset());
			if (Input.justPressed(MouseKeys.WHEEL_DOWN))
				zoomIn();
			if (Input.justPressed(MouseKeys.WHEEL_UP))
				zoomOut();
		}
	}
}
