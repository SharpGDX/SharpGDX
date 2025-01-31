﻿using System.Reflection;
using System.Runtime.CompilerServices;
using SharpGDX.Shims;

namespace SharpGDX.Utils.Reflect
{
    /** Provides information about, and access to, a single field of a class or interface.
 * @author nexsoftware */
    public sealed class Field
    {

        private readonly FieldInfo field;

        internal Field(FieldInfo field)
        {
            this.field = field;
        }

        /** Returns the name of the field. */
        public String getName()
        {
            return field.Name;
        }

        /** Returns a Class object that identifies the declared type for the field. */
        public Type getType()
        {
            return field.FieldType;
        }

        /** Returns the Class object representing the class or interface that declares the field. */
        public Type getDeclaringClass()
        {
            return field.DeclaringType;
        }

        public bool isAccessible()
        {
            return field.IsPublic || field.IsFamily || field.IsAssembly;
        }

        public void setAccessible(bool accessible)
        {
            throw new NotImplementedException();
            //field.setAccessible(accessible);
        }

        /** Return true if the field does not include any of the {@code private}, {@code protected}, or {@code public} modifiers. */
        public bool isDefaultAccess()
        {
            return !isPrivate() && !isProtected() && !isPublic();
        }

        /** Return true if the field includes the {@code final} modifier. */
        public bool isFinal()
        {
            throw new NotImplementedException();
            //return Modifier.isFinal(field.getModifiers());
        }

        /** Return true if the field includes the {@code private} modifier. */
        public bool isPrivate()
        {
            throw new NotImplementedException();
            //return Modifier.isPrivate(field.getModifiers());
        }

        /** Return true if the field includes the {@code protected} modifier. */
        public bool isProtected()
        {
            throw new NotImplementedException();
            //return Modifier.isProtected(field.getModifiers());
        }

        /** Return true if the field includes the {@code public} modifier. */
        public bool isPublic()
        {
            throw new NotImplementedException();
            //return Modifier.isPublic(field.getModifiers());
        }

        /** Return true if the field includes the {@code static} modifier. */
        public bool isStatic()
        {
            return field.IsStatic;
        }

        /** Return true if the field includes the {@code transient} modifier. */
        public bool isTransient()
        {
            // TODO: This is obsolete. -RP
            return field.IsNotSerialized;
        }

        /** Return true if the field includes the {@code volatile} modifier. */
        public bool isVolatile()
        {
            return field.GetRequiredCustomModifiers().Any(f => f == typeof(IsVolatile));
        }

        /** Return true if the field is a synthetic field. */
        public bool isSynthetic()
        {
            // TODO: Is this even possible in C#? -RP
            return field.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }

        /** If the type of the field is parameterized, returns the Class object representing the parameter type at the specified index,
         * null otherwise. */
        public Type getElementType(int index)
        {
            // TODO: Is this correct or do we need modified type? -RP
            Type genericType = field.FieldType;
            if (genericType.IsGenericParameter)
            {
                Type[] actualTypes = (genericType).GetGenericArguments();
                if (actualTypes.Length - 1 >= index)
                {
                    Type actualType = actualTypes[index];
                    //if (actualType is Class)
                    //	return (Class)actualType;
                    //else if (actualType is ParameterizedType)
                    //	return (Type)((ParameterizedType)actualType).getRawType();
                    //else if (actualType is GenericArrayType)
                    //{
                    //	Type componentType = ((GenericArrayType)actualType).getGenericComponentType();
                    //	if (componentType is Class) return ArrayReflection.newInstance((Type)componentType, 0).getClass();
                    //}
                }
            }

            return null;
        }

        internal Type getFieldType()
        {
            // TODO: This entire method is just a stub to try to get JSON parsing to work. -RP
            return field.FieldType;
        }

        /** Returns true if the field includes an annotation of the provided class type. */
        public bool isAnnotationPresent(Type annotationType)
        {
            return Attribute.IsDefined(field, annotationType);
        }

        /** Returns an array of {@link Annotation} objects reflecting all annotations declared by this field, or an empty array if
         * there are none. Does not include inherited annotations. */
        public Annotation[] getDeclaredAnnotations()
        {
            throw new NotImplementedException();
            //	Attribute[] annotations = field.getDeclaredAnnotations();
            //Annotation[] result = new Annotation[annotations.Length];
            //for (int i = 0; i < annotations.Length; i++) {
            //	result[i] = new Annotation(annotations[i]);
            //}
            //return result;
        }

        /** Returns an {@link Annotation} object reflecting the annotation provided, or null of this field doesn't have such an
         * annotation. This is a convenience function if the caller knows already which annotation type he's looking for. */
        public Annotation getDeclaredAnnotation(Type annotationType)
        {
            throw new NotImplementedException();
            //Attribute[] annotations = field.getDeclaredAnnotations();
            //if (annotations == null) {
            //	return null;
            //}
            //foreach (Attribute annotation in annotations) {
            //	if (annotation.annotationType().equals(annotationType)) {
            //		return new Annotation(annotation);
            //	}
            //}
            //return null;
        }

        /** Returns the value of the field on the supplied object. */
        public Object get(Object obj) // TODO: throws ReflectionException 
        {
            try
            {
                return field.GetValue(obj);
            }
            catch (IllegalArgumentException e)
            {
                throw new ReflectionException("Object is not an instance of " + getDeclaringClass(), e);
            }
            // TODO: What exception type is correct?
            //catch (IllegalAccessException e)
            //{
            //	throw new ReflectionException("Illegal access to field: " + getName(), e);
            //}
        }

        /** Sets the value of the field on the supplied object. */
        public void set(Object obj, Object value) // TODO: throws ReflectionException
        {
            try
            {
                field.SetValue(obj, value);
            }
            catch (IllegalArgumentException e)
            {
                throw new ReflectionException("Argument not valid for field: " + getName(), e);
            }
            // TODO: What exception type is correct?
            //catch (IllegalAccessException e)
            //{
            //	throw new ReflectionException("Illegal access to field: " + getName(), e);
            //}
        }

    }
}